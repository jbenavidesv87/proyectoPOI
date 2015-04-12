﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatPOI
{
    public partial class WindowContacts : Form
    {
        public WindowContacts()
        {
            InitializeComponent();
            ClientConection.ConnectToServer(globals.username);
            textBoxUserName.Text = globals.username;
            comboBoxUserStatus.SelectedIndex = 0;
            globals.receivedText = "$$$$";
        }

        private void linkLabelLogout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClientConection.Exit();
            Application.Restart();
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell oneCell in dataGridViewContacts.SelectedCells)
            {
                if (oneCell.Selected)
                    dataGridViewContacts.Rows.RemoveAt(oneCell.RowIndex);
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSearch.Text != "Buscar contacto...")
            {
                dataGridViewContacts.ClearSelection(); //this will clear any currently selected cells
                foreach (DataGridViewRow r in dataGridViewContacts.Rows)
                {
                    r.Visible = true;
                    if (!r.Cells[1].Value.ToString().ToLower().Contains(textBoxSearch.Text.ToLower()))
                    {
                        r.Visible = false;
                    }
                }
            }
        }

        private void dataGridViewContacts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            bool isCreated = false;
            foreach (FormChat temp in Application.OpenForms.OfType<FormChat>())
            {
                if (temp.Text == dataGridViewContacts.Rows[e.RowIndex].Cells[1].Value.ToString())
                {
                    isCreated = true;
                    temp.Focus();
                }
            }
            if (!isCreated)
            {
                FormChat f = new FormChat(dataGridViewContacts.Rows[e.RowIndex].Cells[1].Value.ToString());
                f.Show();
            }

        }

        private void WindowContacts_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClientConection.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (globals.receivedText.Substring(0, 4) == "$cl$")
            {
                dataGridViewContacts.Rows.Clear();
                string[] clientsConnected = globals.receivedText.Substring(4).Split(',');

                foreach (string s in clientsConnected)
                {
                    if (s != globals.username && s != "")
                    {
                        dataGridViewContacts.Rows.Add(new object[] { "Disponible", s, "Mensaje" });
                    }
                }
                ClientConection.SendString("$cs$" + globals.username + "$cs$" + comboBoxUserStatus.Text + ',');
            }

            else if (globals.receivedText.Substring(0, 4) == "$mr$")
            {
                string userFrom = globals.receivedText.Substring(4);
                userFrom = userFrom.Substring(0, userFrom.IndexOf("$mr$"));
                string message = globals.receivedText.Substring(4);
                message = message.Substring(message.IndexOf("$mr$") + 4);

                if (message == "te ha enviado un zumbido.")
                {
                    foreach (FormChat f in Application.OpenForms.OfType<FormChat>())
                    {
                        if (f.Text == userFrom)
                        {
                            f.zumbido();
                            f.setMessage("\n" + userFrom + " " + message);
                        }
                    }
                }
                else
                {

                    foreach (DataGridViewRow dg in dataGridViewContacts.Rows)
                    {
                        if (dg.Cells[1].Value.ToString() == userFrom)
                            dg.Cells[2].Value = message;
                    }

                    foreach (FormChat f in Application.OpenForms.OfType<FormChat>())
                    {
                        if (f.Text == userFrom)
                            f.setMessage("\n" + userFrom + ": " + message);
                    }
                }
            }

            else if (globals.receivedText.Substring(0, 4) == "$cs$")
            {
                string[] variousMessages = globals.receivedText.Split(',');

                foreach (string s in variousMessages)
                {
                    if (s.Length > 4)
                    {
                        string userFrom = s.Substring(4);
                        userFrom = userFrom.Substring(0, userFrom.IndexOf("$cs$"));
                        string status = s.Substring(4);
                        status = status.Substring(status.IndexOf("$cs$") + 4);

                        foreach (DataGridViewRow dg in dataGridViewContacts.Rows)
                        {
                            if (dg.Cells[1].Value.ToString() == userFrom)
                                dg.Cells[0].Value = status;
                        }
                    }
                }
            }

            else if (globals.receivedText.Substring(0, 4) == "$gm$")
            {
                string userFrom = globals.receivedText.Substring(4);
                userFrom = userFrom.Substring(0, userFrom.IndexOf("$gm$"));
                string message = globals.receivedText.Substring(4);
                message = message.Substring(message.IndexOf("$gm$") + 4);

                foreach (FormChat f in Application.OpenForms.OfType<FormChat>())
                {
                    if (f.Text == userFrom)
                        f.setMessage(message);
                }
            }

            else
                return;

            globals.receivedText = "$$$$";
        }

        private void comboBoxUserStatus_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ClientConection.SendString("$cs$" + globals.username + "$cs$" + comboBoxUserStatus.Text + ",");
        }
    }
}
