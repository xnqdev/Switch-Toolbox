﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace LayoutBXLYT
{
    public partial class PaneEditor : LayoutDocked
    {
        public EventHandler PropertyChanged;

        public PaneEditor()
        {
            InitializeComponent();
            stToolStrip1.HighlightSelectedTab = true;
            stToolStrip1.ItemClicked += tabMenu_SelectedIndexChanged;
        }

        public Dictionary<string, STGenericTexture> GetTextures()
        {
            return ParentEditor.GetTextures();
        }

        public List<BasePane> SelectedPanes
        {
            get { return ParentEditor.SelectedPanes; }
        }

        public void Reset()
        {
            stToolStrip1.Items.Clear();
            stPanel1.Controls.Clear();
        }

        public void ReloadEditor()
        {
            if (ActivePane != null)
                LoadPane(ActivePane, ParentEditor);
        }

        public BxlanHeader ActiveAnimation;

        private LayoutEditor ParentEditor;
        private BxlytMaterial ActiveMaterial;
        private BasePane ActivePane;
        private bool Loaded = false;
        private bool MaterialMode = false;

        private Control ActiveEditor
        {
            get
            {
                if (stPanel1.Controls.Count == 0) return null;
                return stPanel1.Controls[0];
            }
        }

        private STPropertyGrid propertyGrid;
        public void LoadProperties(object properties)
        {
            Reset();

            MaterialMode = false;

            AddTab("Properties");

            if (propertyGrid == null || propertyGrid.Disposing || propertyGrid.IsDisposed)
            {
                propertyGrid = new STPropertyGrid();
                propertyGrid.Dock = DockStyle.Fill;
                stPanel1.Controls.Add(propertyGrid);
            }

            propertyGrid.LoadProperty(properties, ProperyChanged);
        }

        private void ProperyChanged()
        {
            PropertyChanged?.Invoke(propertyGrid, EventArgs.Empty);
        }

        public void RefreshEditor()
        {
            var editor = ActiveEditor;
            if (editor == null) return;

            if (editor is BasePaneEditor)
                ((BasePaneEditor)editor).RefreshEditor();
        }

        public void LoadMaterial(BxlytMaterial material, LayoutEditor parentEditor)
        {
            ActiveMaterial = material;
            ParentEditor = parentEditor;

            Loaded = false;
            MaterialMode = true;

            stToolStrip1.Items.Clear();

            AddTab("Texture Maps", LoadTextureMaps);
            AddTab("Colors", LoadColorBlending);
            AddTab("Blending", LoadBlending);
            AddTab("Combiners", LoadTextureCombiners);

            stToolStrip1.Items[Runtime.LayoutEditor.MaterialTabIndex].PerformClick();

            Loaded = true;
        }

        public void LoadPane(BasePane pane, LayoutEditor parentEditor)
        {
            ParentEditor = parentEditor;

            Loaded = false;
            MaterialMode = false;

            ActivePane = pane;

            stToolStrip1.Items.Clear();
            AddTab("Pane", LoadBasePane);
            if (pane is IPicturePane)
            {
                ActiveMaterial = ((IPicturePane)ActivePane).Material;
                AddTab("Picture Pane", LoadPicturePane);
                AddTab("Texture Maps", LoadTextureMaps);
                AddTab("Colors", LoadColorBlending);
                AddTab("Blending", LoadBlending);
                AddTab("Combiners", LoadTextureCombiners);
            }
            if (pane is IWindowPane)
            {
                //Note active material is set in window pane editor based on frame selector

                AddTab("Window Pane", LoadWindowPane);
                AddTab("Texture Maps", LoadWindowLoadTextureMaps);
                AddTab("Colors", LoadWindowColorBlending);
                AddTab("Blending", LoadWindowMatBlending);
                AddTab("Combiners", LoadWindowTextureCombiners);
            }
            if (pane is ITextPane)
            {
                ActiveMaterial = ((ITextPane)ActivePane).Material;

                AddTab("Text Pane", LoadTextPane);
                AddTab("Colors", LoadColorBlending);
                AddTab("Blending", LoadBlending);
            }
            if (pane is IPartPane)
            {
                AddTab("Part Pane", LoadPartPane);
            }

            AddTab("User Data", LoadUserData);

            int tabIndex;
            if (pane is IPicturePane)
                tabIndex = Runtime.LayoutEditor.PicturePaneTabIndex;
            else if (pane is IWindowPane)
                tabIndex = Runtime.LayoutEditor.WindowPaneTabIndex;
            else if (pane is ITextPane)
                tabIndex = Runtime.LayoutEditor.TextPaneTabIndex;
            else
                tabIndex = Runtime.LayoutEditor.NullPaneTabIndex;

            stToolStrip1.Items[tabIndex].PerformClick();

            Loaded = true;
        }

        public void UpdateTextureList() {
            ParentEditor.UpdateLayoutTextureList();
        }

        private void AddTab(string name, EventHandler eventHandler = null, Image image = null)
        {
            stToolStrip1.Items.Add(new STToolStipMenuButton(name, image, eventHandler)
            {
                DisplayStyle = ToolStripItemDisplayStyle.Text,
            });
        }

        //Default to the previously used editor if the editor is closed and reopened
        private void UpdateTabIndex()
        {
            if (ActivePane == null && !MaterialMode || !Loaded) return;

            int tabIndex = stToolStrip1.SelectedTabIndex;

            if (MaterialMode)
                Runtime.LayoutEditor.MaterialTabIndex = tabIndex;
            else if (ActivePane is IPicturePane)
                Runtime.LayoutEditor.PicturePaneTabIndex = tabIndex;
            else if (ActivePane is IWindowPane)
                Runtime.LayoutEditor.WindowPaneTabIndex = tabIndex;
            else if (ActivePane is ITextPane)
                Runtime.LayoutEditor.TextPaneTabIndex = tabIndex;
            else
               Runtime.LayoutEditor.NullPaneTabIndex = tabIndex;
        }

        private void LoadUserData(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var userEditor = GetActiveEditor<UserDataEditor>();
            if (ActivePane is IUserDataContainer)
                userEditor.LoadUserData(ActivePane, ((IUserDataContainer)ActivePane).UserData);
        }

        private void LoadTextureMaps(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var textureEditor = GetActiveEditor<PaneMatTextureMapsEditor>();
            textureEditor.LoadMaterial(ActiveMaterial, this, ParentEditor.GetTextures());
        }

        private void LoadPartPane(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var partEditor = GetActiveEditor<PartPaneEditor>();
            partEditor.LoadPane(ActivePane as IPartPane, this);
        }

        private void LoadColorBlending(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var colorEditor = GetActiveEditor<PaneMatColorEditor>();
            colorEditor.LoadMaterial(ActiveMaterial, this);
        }

        private void LoadBlending(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var blendEditor = GetActiveEditor<PaneMatBlending>();
            blendEditor.LoadMaterial(ActiveMaterial, this);
        }

        private void LoadTextureCombiners(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var texCombEditor = GetActiveEditor<PaneMatTextureCombiner>();
            texCombEditor.LoadMaterial(ActiveMaterial, this);
        }

        private void LoadTextPane(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var pictureEditor = GetActiveEditor<PaneTextBoxEditor>();
            pictureEditor.LoadPane(ActivePane as ITextPane, this);
        }

        private void LoadPicturePane(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var pictureEditor = GetActiveEditor<BasePictureboxEditor>();
            pictureEditor.LoadPane(ActivePane as IPicturePane, this);
        }

        private void LoadWindowLoadTextureMaps(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var windowEditor = GetActiveEditor<WindowPaneEditor>();
            windowEditor.LoadPane(ActivePane as IWindowPane,
                WindowPaneEditor.ContentType.Textures, this);
        }

        private void LoadWindowColorBlending(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var windowEditor = GetActiveEditor<WindowPaneEditor>();
            windowEditor.LoadPane(ActivePane as IWindowPane,
                WindowPaneEditor.ContentType.ColorInterpolation, this);
        }

        private void LoadWindowMatBlending(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var windowEditor = GetActiveEditor<WindowPaneEditor>();
            windowEditor.LoadPane(ActivePane as IWindowPane,
                WindowPaneEditor.ContentType.Blending, this);
        }

        private void LoadWindowTextureCombiners(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var windowEditor = GetActiveEditor<WindowPaneEditor>();
            windowEditor.LoadPane(ActivePane as IWindowPane,
                WindowPaneEditor.ContentType.TextureCombiners, this);
        }

        private void LoadWindowPane(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var windowEditor = GetActiveEditor<WindowPaneEditor>();
            windowEditor.LoadPane(ActivePane as IWindowPane,
                WindowPaneEditor.ContentType.WindowContent, this);
        }

        private void LoadBasePane(object sender, EventArgs e)
        {
            UpdateTabIndex();
            var basePaneEditor = GetActiveEditor<BasePaneEditor>();
            basePaneEditor.LoadPane(ActivePane, this);
        }

        private T GetActiveEditor<T>() where T : Control, new()
        {
            T instance = new T();

            if (ActiveEditor?.GetType() == instance.GetType())
                return ActiveEditor as T;
            else
            {
                DisposeEdtiors();
                stPanel1.Controls.Clear();
                instance.Dock = DockStyle.Fill;
                stPanel1.Controls.Add(instance);
            }

            return instance;
        }

        private void DisposeEdtiors()
        {
            if (ActiveEditor == null) return;
            if (ActiveEditor is STUserControl)
                ((STUserControl)ActiveEditor).OnControlClosing();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            DisposeEdtiors();
        }

        private void tabMenu_SelectedIndexChanged(object sender, EventArgs e) {
        }
    }
}
