using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.Audit;

namespace COTES.ISTOK.Assignment.Audit
{
    class UnitTypeManagerAuditDecorator : IUnitTypeManager
    {
        IUnitTypeManager typeManager;
        IAuditServer audit;

        public ISecurityManager Security { get; set; }

        public UnitTypeManagerAuditDecorator(IAuditServer auditServer, IUnitTypeManager typeManager)
        {
            this.audit = auditServer;
            this.typeManager = typeManager;
        }

        private void AuditChangeType(AuditEntry entry, UTypeNode oldType, UTypeNode newType)
        {
            if (oldType == null
                || newType == null
                || !String.Equals(oldType.Text, newType.Text)
                || !String.Equals(oldType.Filter, newType.Filter)
                || !String.Equals(oldType.Props, newType.Props)
                || !EqualsBinaries(oldType.Icon, newType.Icon))
            {
                entry.AuditTypes.Add(new AuditType()
                {
                    TypeID = (newType ?? oldType).Idnum,
                    ExtGuid = (newType ?? oldType).ExtensionGUID,
                    TypeNameOld = (oldType == null ? null : oldType.Text),
                    TypeNameNew = (newType == null ? null : newType.Text),
                    TypeChildFilterOld = (oldType == null ? null : oldType.Filter),
                    TypeChildFilterNew = (newType == null ? null : newType.Filter),
                    TypePropsOld = (oldType == null ? null : oldType.Props),
                    TypePropsNew = (newType == null ? null : newType.Props),
                    TypeImageOld = (oldType == null ? null : oldType.Icon),
                    TypeImageNew = (newType == null ? null : newType.Icon)
                });
            }
        }

        private bool EqualsBinaries(byte[] oldValue, byte[] newValue)
        {
            if (oldValue == null && newValue == null)
            {
                return true;
            }
            if (oldValue == null || newValue == null)
            {
                return false;
            }
            if (oldValue.Length != newValue.Length)
            {
                return false;
            }
            for (int i = 0; i < oldValue.Length; i++)
            {
                if (!byte.Equals(oldValue[i], newValue[i]))
                {
                    return false;
                }
            }
            return true;
        }

        #region Audited IUnitTypeManager Members

        public UTypeNode AddUnitType(OperationState state, UTypeNode unitTypeNode)
        {
            var typeNode = typeManager.AddUnitType(state, unitTypeNode);

            var entry = new AuditEntry(Security.GetUserInfo(state.UserGUID));

            AuditChangeType(entry, null, typeNode);

            audit.WriteAuditEntry(entry);

            return typeNode;
        }

        public UTypeNode UpdateUnitType(OperationState state, UTypeNode unitTypeNode)
        {
            var oldType = typeManager.GetUnitType(state, unitTypeNode.Idnum).Clone() as UTypeNode;

            var typeNode = typeManager.UpdateUnitType(state, unitTypeNode);

            var entry = new AuditEntry(Security.GetUserInfo(state.UserGUID));

            AuditChangeType(entry, oldType, typeNode);

            audit.WriteAuditEntry(entry);

            return typeNode;
        }

        public void RemoveUnitType(OperationState state, int unitTypeNodeID)
        {
            var typeNode = typeManager.GetUnitType(state, unitTypeNodeID).Clone() as UTypeNode;

            typeManager.RemoveUnitType(state, unitTypeNodeID);

            var entry = new AuditEntry(Security.GetUserInfo(state.UserGUID));

            AuditChangeType(entry, typeNode, null);

            audit.WriteAuditEntry(entry);
        }
        #endregion

        #region Rest IUnitTypeManager Members

        public void LoadTypes(OperationState state)
        {
            typeManager.LoadTypes(state);
        }

        public UTypeNode GetUnitType(OperationState state, int type)
        {
            return typeManager.GetUnitType(state, type);
        }

        public UTypeNode[] GetUnitTypes(OperationState state)
        {
            return typeManager.GetUnitTypes(state);
        }

        public int GetExtensionType(Guid parentGUID)
        {
            return typeManager.GetExtensionType(parentGUID);
        }

        public event EventHandler<UnitTypeEventArgs> UnitTypeChanged
        {
            add { typeManager.UnitTypeChanged += value; }
            remove { typeManager.UnitTypeChanged -= value; }
        }

        #endregion
    }
}
