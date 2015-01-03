using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    class UserInterfaceServiceContainer : ISite, IUserInterfaceRequest
    {
        Session session;
        ISite baseContainer;

        public UserInterfaceServiceContainer(ISite container, Session session)
        {
            baseContainer = container;
            this.session = session;
        }

        #region ISite Members

        public IComponent Component
        {
            get
            {
                if (baseContainer == null)
                {
                    return null;
                }
                return baseContainer.Component;
            }
        }

        public IContainer Container
        {
            get
            {
                if (baseContainer == null)
                {
                    return null;
                }
                return baseContainer.Container;
            }
        }

        public bool DesignMode
        {
            get { return false; }
        }

        public string Name { get; set; }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType.IsInstanceOfType(this))
            {
                return this;
            }
            if (baseContainer == null)
            {
                return null;
            }
            return baseContainer.GetService(serviceType);
        }

        #endregion

        #region IUserInterfaceRequest Members

        public UnitNode PromtUnitNode(UnitNode unitNode)
        {
            SelectForm form = new SelectForm(session.GetStructureProvider());
            form.MultiSelect = false;
            form.ShowDialog();
            if (form.SelectedObjects != null && form.SelectedObjects.Length > 0)
            {
                return form.SelectedObjects[0];
                //Table.RootNodeID = unitNode.Idnum;
                //rootNodeTextBox.Text = unitNode.FullName;
            }
            else
            {
                return null;
                //Table.RootNodeID = 0;
                //rootNodeTextBox.Text = String.Empty;
            }
        }

        public IEnumerable<UnitNode> SelectNodes(string caption, IEnumerable<UnitNode> beginNodes)
        {
            ExtendSelectForm form = new ExtendSelectForm(session.GetStructureProvider());
            form.Filter.Add((int)UnitTypeId.Parameter);
            form.Filter.Add((int)UnitTypeId.ManualParameter);
            form.Filter.Add((int)UnitTypeId.TEP);

            if (beginNodes != null)
            {
                var prov = this.GetService(typeof(IStructureRetrieval)) as IStructureRetrieval;

                form.SelectedObjects = (from u in beginNodes select prov.GetUnitNode(u.Idnum)).ToArray();
            }

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return form.SelectedObjects;
            }
            return beginNodes;
        }

        public int[] PromtUnitTypesDropDown(IServiceProvider provider, int[] beginTypes)
        {
            var typeRetrieval = GetService(typeof(IUnitTypeRetrieval)) as IUnitTypeRetrieval;

            if (typeRetrieval != null && provider != null)
            {
                IWindowsFormsEditorService svc =
                  (IWindowsFormsEditorService)
                  provider.GetService(typeof(IWindowsFormsEditorService));

                if (svc != null)
                {
                    var flctrl = new Control();
                    flctrl.Tag = svc;

                    var types = typeRetrieval.GetUnitTypes(0);
                    HashSet<int> startTypes = new HashSet<int>(beginTypes ?? new int[] { });

                    ImageList imageList = new ImageList();

                    int defaultWidth = 120;
                    int extraHeight = 6;
                    int x = 0, y = 0;
                    foreach (var id in types)
                    {
                        int height;
                        var typeNode = typeRetrieval.GetUnitTypeNode(id);

                        using (var stream = new MemoryStream(typeNode.Icon))
                        {
                            var img = Image.FromStream(stream);
                            height = img.Height + extraHeight;
                            imageList.Images.Add(typeNode.Idnum.ToString(), img);
                        }

                        flctrl.Controls.Add(new CheckBox()
                        {
                            Location = new System.Drawing.Point(x, y),
                            Tag = id,
                            Text = typeNode.Text,
                            Size = new Size(defaultWidth, height),
                            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                            Checked = startTypes != null && startTypes.Contains(id),
                            ImageList = imageList,
                            ImageKey = id.ToString(),
                            ImageAlign = ContentAlignment.MiddleLeft,
                            TextImageRelation = TextImageRelation.ImageBeforeText
                        });
                        y += height;
                    }

                    flctrl.Size = new System.Drawing.Size(defaultWidth, y);

                    svc.DropDownControl(flctrl);

                    return flctrl.Controls.OfType<CheckBox>().Where(c => c.Checked).Select(c => (int)c.Tag).ToArray();
                }
            }
            return beginTypes;
        }
        #endregion
    }

}
