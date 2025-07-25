using System.Globalization;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;

namespace Application.Services;

public interface ISubscriptionPaymentStatusService
{
    Task CreateInitialStatusAsync(Guid correlationId, string userId, Guid subscriptionId, decimal amount,
        string currency, string orderId, CancellationToken cancellationToken = default);

    Task UpdatePaymentUrlCreatedAsync(Guid correlationId, string paymentUrl, string urlCreatedAt,
        CancellationToken cancellationToken = default);

    Task UpdatePaymentUrlFailedAsync(Guid correlationId, string reason, CancellationToken cancellationToken = default);

    Task UpdatePaymentCompletedAsync(Guid correlationId, string transactionId, DateTime completedAt,
        CancellationToken cancellationToken = default);

    Task UpdatePaymentFailedAsync(Guid correlationId, string reason, DateTime failedAt,
        CancellationToken cancellationToken = default);

    Task UpdateSubscriptionActivatedAsync(Guid correlationId, CancellationToken cancellationToken = default);

    Task UpdateSubscriptionActivationFailedAsync(Guid correlationId, string reason,
        CancellationToken cancellationToken = default);
}

public class SubscriptionPaymentStatusService : ISubscriptionPaymentStatusService
{
    private readonly ISubscriptionPaymentStatusRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubscriptionPaymentStatusService> _logger;

    public SubscriptionPaymentStatusService(
        ISubscriptionPaymentStatusRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<SubscriptionPaymentStatusService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task CreateInitialStatusAsync(Guid correlationId, string userId, Guid subscriptionId, decimal amount,
        string currency, string orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var existing = await _repository.GetByCorrelationIdAsync(correlationId, cancellationToken);
            if (existing != null)
            {
                _logger.LogWarning("SubscriptionPaymentStatus already exists for CorrelationId {CorrelationId}",
                    correlationId);
                return;
            }

            var status = new SubscriptionPaymentStatus
            {
                Id = Guid.NewGuid(),
                CorrelationId = correlationId,
                UserId = userId,
                SubscriptionId = subscriptionId,
                Amount = amount,
                Currency = currency,
                OrderId = orderId,
                CurrentState = "PaymentUrlCreating",
                PaymentUrlCreated = false,
                PaymentCompleted = false,
                SubscriptionActivated = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(status, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created initial payment status for CorrelationId {CorrelationId}", correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating initial payment status for CorrelationId {CorrelationId}",
                correlationId);
        }
    }

    public async Task UpdatePaymentUrlCreatedAsync(Guid correlationId, string paymentUrl, string urlCreatedAt,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await _repository.GetByCorrelationIdAsync(correlationId, cancellationToken);
            if (status == null)
            {
                _logger.LogWarning("SubscriptionPaymentStatus not found for CorrelationId {CorrelationId}",
                    correlationId);
                return;
            }

            status.PaymentUrl = paymentUrl;
            status.PaymentUrlCreated = true;
            status.CurrentState = "PaymentPending";
            status.UpdatedAt = DateTime.UtcNow;
            status.TransactionId = urlCreatedAt;
            
            _repository.Update(status);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated payment URL created for CorrelationId {CorrelationId}", correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment URL created for CorrelationId {CorrelationId}", correlationId);
        }
    }

    public async Task UpdatePaymentUrlFailedAsync(Guid correlationId, string reason,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await _repository.GetByCorrelationIdAsync(correlationId, cancellationToken);
            if (status == null)
            {
                _logger.LogWarning("SubscriptionPaymentStatus not found for CorrelationId {CorrelationId}",
                    correlationId);
                return;
            }

            status.CurrentState = "Failed";
            status.FailureReason = reason;
            status.UpdatedAt = DateTime.UtcNow;

            _repository.Update(status);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated payment URL failed for CorrelationId {CorrelationId}", correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment URL failed for CorrelationId {CorrelationId}", correlationId);
        }
    }

    public async Task UpdatePaymentCompletedAsync(Guid correlationId, string transactionId, DateTime completedAt,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await _repository.GetByCorrelationIdAsync(correlationId, cancellationToken);
            if (status == null)
            {
                _logger.LogWarning("SubscriptionPaymentStatus not found for CorrelationId {CorrelationId}",
                    correlationId);
                return;
            }

            status.PaymentCompleted = true;
            status.TransactionId = transactionId;
            status.CompletedAt = completedAt;
            status.CurrentState = "SubscriptionActivating";
            status.UpdatedAt = DateTime.UtcNow;

            _repository.Update(status);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated payment completed for CorrelationId {CorrelationId}", correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment completed for CorrelationId {CorrelationId}", correlationId);
        }
    }

    public async Task UpdatePaymentFailedAsync(Guid correlationId, string reason, DateTime failedAt,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await _repository.GetByCorrelationIdAsync(correlationId, cancellationToken);
            if (status == null)
            {
                _logger.LogWarning("SubscriptionPaymentStatus not found for CorrelationId {CorrelationId}",
                    correlationId);
                return;
            }

            status.CurrentState = "Failed";
            status.FailureReason = reason;
            status.UpdatedAt = DateTime.UtcNow;

            _repository.Update(status);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated payment failed for CorrelationId {CorrelationId}", correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment failed for CorrelationId {CorrelationId}", correlationId);
        }
    }

    public async Task UpdateSubscriptionActivatedAsync(Guid correlationId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await _repository.GetByCorrelationIdAsync(correlationId, cancellationToken);
            if (status == null)
            {
                _logger.LogWarning("SubscriptionPaymentStatus not found for CorrelationId {CorrelationId}",
                    correlationId);
                return;
            }

            status.SubscriptionActivated = true;
            status.CurrentState = "Completed";
            status.UpdatedAt = DateTime.UtcNow;

            _repository.Update(status);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated subscription activated for CorrelationId {CorrelationId}", correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subscription activated for CorrelationId {CorrelationId}",
                correlationId);
        }
    }

    public async Task UpdateSubscriptionActivationFailedAsync(Guid correlationId, string reason,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await _repository.GetByCorrelationIdAsync(correlationId, cancellationToken);
            if (status == null)
            {
                _logger.LogWarning("SubscriptionPaymentStatus not found for CorrelationId {CorrelationId}",
                    correlationId);
                return;
            }

            status.CurrentState = "Failed";
            status.FailureReason = reason;
            status.UpdatedAt = DateTime.UtcNow;

            _repository.Update(status);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated subscription activation failed for CorrelationId {CorrelationId}",
                correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subscription activation failed for CorrelationId {CorrelationId}",
                correlationId);
        }
    }
}