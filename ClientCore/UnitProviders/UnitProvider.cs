using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    public class UnitProvider : IUnitNodeProvider, IDisposable
    {
        protected RemoteDataService RDS { get { return strucProvider.RDS; } }
        protected StructureProvider strucProvider;

        protected UnitNode unitNode;
        protected UnitNode newUnitNode;

        internal UnitProvider(StructureProvider strucProvider, UnitNode unitNode)
        {
            this.strucProvider = strucProvider;
            this.unitNode = unitNode;
            this.StructureProvider.CurrentRevisionChanged += StructureProvider_CurrentRevisionChanged;
        }

        void StructureProvider_CurrentRevisionChanged(object sender, EventArgs e)
        {
            OnCurrentRevisionChanged();
        }

        #region Properties
        public StructureProvider StructureProvider { get { return strucProvider; } }
        public UnitNode UnitNode { get { return unitNode; } }
        /// <summary>
        /// Обновленный узел
        /// </summary>
        public virtual UnitNode NewUnitNode
        {
            get
            {
                if (newUnitNode == null && unitNode != null)
                    newUnitNode = unitNode.Clone() as UnitNode;
                return newUnitNode;
            }
            set
            {
                newUnitNode = value;
                OnNewUnitNodeChanged();
            }
        }

        bool editMode;
        
        public virtual bool EditMode
        {
            get { return editMode; }
            set
            {
                editMode = value;
                OnEditModeChanged();
            }
        }        

        /// <summary>
        /// Данный узел может редактироваться
        /// </summary>
        public bool Editable
        {
            get { return RDS.NodeDataService.CheckAccess(strucProvider.Session.User, unitNode, Privileges.Write); }
        }

        /// <summary>
        /// Компоненты открываются только на чтение
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return !Editable || !EditMode;
            }
        }

        /// <summary>
        /// Данные изменились
        /// </summary>
        public virtual bool HasChanges
        {
            get { return !unitNode.Equals(NewUnitNode); }
        }
        #endregion

        #region IUnitNodeProvider Members
        public RevisionInfo CurrentRevision
        {
            get { return strucProvider.CurrentRevision; }
        }

        public RevisionInfo GetRealRevision(RevisionInfo revision)
        {
            if (RevisionInfo.Current.Equals(revision))
                return CurrentRevision;
            if (RevisionInfo.Head.Equals(revision))
                return unitNode.GetHeadRevision();

            return revision;
        }
        #endregion

        public virtual void LoadWorkInfo()
        {
            //
        }

        public virtual void Lock()
        {
            //try
            //{
            strucProvider.LockNode(UnitNode);
            EditMode = true;
            //}
            //catch (LockException exc)
            //{
            //    throw exc;
            //    //if (UniForm.LockException(UnitNode, exc))
            //    //    Lock();
            //}
            //catch (UnitNodeNotExistException)
            //{
            //    throw;
            //    //UniForm uniForm = UniForm as UniForm;
            //    //if (uniForm != null)
            //    //{
            //    //    uniForm.NodeNotFoundNotify(UnitNode);
            //    //}
            //}
        }

        public virtual string GetConfirmMessage()
        {
            if (unitNode != null && !String.IsNullOrEmpty(unitNode.Code) && String.IsNullOrEmpty(NewUnitNode.Code))
            {
                return "Удаление кода параметра приведет к неработоспособности формул.";
            }
            return null;
        }

        public virtual IEnumerable<ActionArgs> Save()
        {
            List<ActionArgs> actions = new List<ActionArgs>();

            bool updateReference = unitNode != null && !String.IsNullOrEmpty(unitNode.Code) && !unitNode.Code.Equals(newUnitNode.Code)
                     && strucProvider.HasReference(unitNode.Code);

            if (updateReference)
            {
                actions.Add(new ChangeParameterCodeActionArgs(unitNode, newUnitNode));
            }

            unitNode = strucProvider.UpdateUnitNode(newUnitNode);
            newUnitNode = null;
            CommitSaving();
            OnNodeSaved();

            return actions.Count > 0 ? actions.ToArray() : null;
        }

        public virtual void CommitSaving()
        {
            //unitNode = newUnitNode;
        }

        public virtual void ClearUnsavedData()
        {
            NewUnitNode = null;
            if (UnitNode != null && EditMode)
                strucProvider.ReleaseNode(UnitNode);
            if (EditMode) EditMode = false;
        }

        protected virtual void DisposeProvider()
        {
            //
        }
        #region IDisposable Members
        public virtual void Dispose()
        {
            this.StructureProvider.CurrentRevisionChanged -= StructureProvider_CurrentRevisionChanged;
            DisposeProvider();
        }
        #endregion

        public Interval MaxInterval { get { return Interval.Zero; } set { } }

        public DateTime MinStartTime { get { return DateTime.MinValue; } set { } }

        public bool Calculable
        {
            get
            {
                //if(FilterParams== FilterParams.All&&FilterParams
                if (strucProvider.FilterParams == FilterParams.TepParameters)
                    //&& strucProvider.FilteredTypes.Length == 1
                    //&& strucProvider.FilteredTypes[0] == (int)UnitTypeId.TEPTemplate)
                {
                    return true;
                }
                //UniForm megaUnitForm = UniForm as UniForm;
                //return megaUnitForm != null && megaUnitForm.CalcForm && CalcNodes.Length > 0;
                return false;
            }
        }

        //public delegate void SaveItemDelegate(UnitNode node);
        //public event SaveItemDelegate OnSave = null;

        #region Events
        public event EventHandler NewUnitNodeChanged;
        public event EventHandler EditModeChanged;
        public event EventHandler CurrentRevisionChanged;
        public event EventHandler NodeSaved;

        public void OnNewUnitNodeChanged()
        {
            if (NewUnitNodeChanged != null)
                NewUnitNodeChanged(this, EventArgs.Empty);
        }
        protected virtual void OnCurrentRevisionChanged()
        {
            if (CurrentRevisionChanged != null)
                CurrentRevisionChanged(this, EventArgs.Empty);
        }
        protected virtual void OnEditModeChanged()
        {
            if (EditModeChanged != null)
                EditModeChanged(this, EventArgs.Empty);
        }
        protected virtual void OnNodeSaved()
        {
            if (NodeSaved != null)
                NodeSaved(this, EventArgs.Empty);
        }
        #endregion
    }
}
