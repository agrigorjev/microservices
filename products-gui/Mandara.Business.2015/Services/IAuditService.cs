using System.Collections.Generic;
using Mandara.Business.Audit;
using Mandara.Entities;

namespace Mandara.Business.Services
{
    public interface IAuditService
    {
        AuditMessage CreateAuditMessage<T>(AuditContext auditContext, string messageType, T originalEntity, T modifiedEntity) where T : class;
        AuditMessage UpdateAuditMessage<T>(AuditMessage auditMessage, T modifiedEntity) where T : class;

        List<AuditMessage> CreateAuditMessages<T>(AuditContext auditContext, string messageType, List<T> originalEntities, List<T> modifiedEntities) where T : class;
        List<AuditMessage> UpdateAuditMessages<T>(List<AuditMessage> auditMessages, List<T> modifiedEntities) where T : class;
        void RemoveEqualDetails(AuditMessageDetails details);
    }
}