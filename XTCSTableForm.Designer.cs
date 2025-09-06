using System;
using System.Windows.Forms;

namespace AdelCardReaderManager
{
    partial class XTCSTableForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // We're creating the controls manually in the XTCSTableForm.cs file
        // No InitializeComponent() method needed
    }
}