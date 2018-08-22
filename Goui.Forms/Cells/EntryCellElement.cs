﻿using System;
using System.ComponentModel;
using Goui.Forms.Extensions;
using Goui.Html;
using Xamarin.Forms;

namespace Goui.Forms.Cells
{
    public class EntryCellElement : CellElement
    {
        public Goui.Html.Label TextLabel { get; } = new Goui.Html.Label ();
        public Goui.Html.TextInput TextInput { get; } = new Goui.Html.TextInput ();

        public EntryCellElement ()
        {
            AppendChild (TextLabel);
            AppendChild (TextInput);

            TextInput.Change += TextInput_Change;
        }

        protected override void BindCell ()
        {
            Cell.PropertyChanged += Cell_PropertyChanged;

            if (Cell is EntryCell cell) {
                UpdateLabel (cell);
                UpdateText (cell);
                UpdatePlaceholder (cell);
                UpdateLabelColor (cell);
            }

            base.BindCell ();
        }

        protected override void UnbindCell ()
        {
            Cell.PropertyChanged -= Cell_PropertyChanged;
            base.UnbindCell ();
        }

        void Cell_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            var entryCell = (EntryCell)sender;

            if (e.PropertyName == EntryCell.LabelProperty.PropertyName)
                UpdateLabel (entryCell);
            else if (e.PropertyName == EntryCell.TextProperty.PropertyName)
                UpdateText (entryCell);
            else if (e.PropertyName == EntryCell.PlaceholderProperty.PropertyName)
                UpdatePlaceholder (entryCell);
            else if (e.PropertyName == EntryCell.LabelColorProperty.PropertyName)
                UpdateLabelColor (entryCell);
        }

        void UpdateLabel (EntryCell entryCell)
        {
            TextLabel.Text = entryCell.Label ?? string.Empty;
        }

        void UpdateLabelColor (EntryCell entryCell)
        {
            TextLabel.Style.Color = entryCell.LabelColor.ToGouiColor (GouiTheme.TextColor);
        }

        void UpdatePlaceholder (EntryCell entryCell)
        {
            TextInput.Placeholder = entryCell.Placeholder ?? string.Empty;
        }

        void UpdateText (EntryCell entryCell)
        {
            TextInput.Value = entryCell.Text ?? string.Empty;
        }

        void TextInput_Change (object sender, EventArgs e)
        {
            if (Cell is EntryCell cell)
                cell.Text = TextInput.Text;
        }
    }
}
