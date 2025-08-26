using System.Data;
using Dapper;
using RoletaBrindes.Application.DTOs;
using RoletaBrindes.Domain.Models;
using RoletaBrindes.Infrastructure.Data;
using RoletaBrindes.Infrastructure.Repositories.Interfaces;

namespace RoletaBrindes.Application.Services;

public class SpinService
{
    private readonly IConnectionFactory _factory;
    private readonly IGiftRepository _gifts;
    private readonly IParticipantRepository _participants;
    private readonly ISpinRepository _spins;
    private readonly Random _rng = new();

    public SpinService(IConnectionFactory factory, IGiftRepository gifts, IParticipantRepository participants, ISpinRepository spins)
    { _factory = factory; _gifts = gifts; _participants = participants; _spins = spins; }

    public async Task<SpinResponse> SpinAsync(string name, string phone)
    {
        using var conn = _factory.NewConnection();
        await conn.OpenAsync();
        using var tx = conn.BeginTransaction(IsolationLevel.RepeatableRead);

        bool hasParticipantWithPhone = await _participants.HasUserWithPhoneAsync(phone, tx);

        if (hasParticipantWithPhone)
        {
            return new SpinResponse
            {
                Won = false,
                Message = "Obrigado por participar, mas este telefone já foi utilizado para o sorteio.",
                Segments = new(),
                TargetIndex = -1
            };
        }
        
        // Upsert participante
        var participantId = await _participants.UpsertByPhoneAsync(name.Trim(), phone.Trim(), tx);

        // Carrega brindes elegíveis e bloqueia as linhas até o commit
        var gifts = (await _gifts.ListActiveAsync(tx)).ToList();

        if (gifts.Count == 0)
        {
            await _spins.InsertAsync(new Spin { Participant_Id = participantId, Gift_Id = null, Won = false }, tx);
            await tx.CommitAsync();
            return new SpinResponse
            {
                Won = false,
                Message = "Obrigado por participar! Infelizmente os brindes esgotaram.",
                Segments = new(),
                TargetIndex = -1
            };
        }

        // Sorteio ponderado
        var totalWeight = gifts.Sum(g => Math.Max(0, g.Weight));
        var pick = _rng.Next(1, totalWeight + 1);
        int cumulative = 0, chosenIndex = 0; Gift chosen = gifts[0];
        for (int i = 0; i < gifts.Count; i++)
        {
            cumulative += Math.Max(0, gifts[i].Weight);
            if (pick <= cumulative) { chosen = gifts[i]; chosenIndex = i; break; }
        }

        // Debitar estoque com checagem otimista (garantida pelo FOR UPDATE implícito na query quando em tx)
        var affected = await conn.ExecuteAsync(
            @"UPDATE gifts
              SET stock = stock - 1,
                  is_active = CASE WHEN stock - 1 <= 0 THEN FALSE ELSE is_active END
              WHERE id = @id AND stock > 0",
            new { id = chosen.Id }, tx);

        if (affected == 0)
        {
            // Estoque foi consumido concorrencialmente; registrar sem prêmio (ou repetir lógica)
            await _spins.InsertAsync(new Spin { Participant_Id = participantId, Gift_Id = null, Won = false }, tx);
            await tx.CommitAsync();
            return new SpinResponse { Won = false, Message = "Tente novamente!", Segments = gifts.Select(g => g.Name).ToList(), TargetIndex = -1 };
        }

        // Registrar spin vencedor
        await _spins.InsertAsync(new Spin { Participant_Id = participantId, Gift_Id = chosen.Id, Won = true }, tx);

        await tx.CommitAsync();

        return new SpinResponse
        {
            Won = true,
            GiftName = chosen.Name,
            Message = $"Você ganhou: {chosen.Name}! Dirija-se à área do Rotaract para retirar seu brinde.",
            Segments = gifts.Select(g => g.Name).ToList(),
            TargetIndex = chosenIndex
        };
    }
}