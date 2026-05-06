using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace HotelManagementSystem.WinForms.Forms;

public class GuestsForm : Form
{
    public string connectionString = "Server=localhost;Database=HotelDb;Trusted_Connection=True;";
    public GuestsForm()
    {

        InitializeComponents();
        //this.Load += GuestsForm_Load;
    }

    private void InitializeComponents()
    {
        backButton = new Button();
        insertButton = new Button();
        updateButton = new Button();
        deleteButton = new Button();
        loadButton = new Button();

        dataGridView1 = new DataGridView();

        firstNameLabel = new Label();
        secondNameLabel = new Label();
        phoneLabel = new Label();
        nationalIDLabel = new Label();

        panel1 = new Panel();
        firstNameText = new TextBox();
        lastNameText = new TextBox();
        phoneText = new TextBox();
        nationalIdText = new TextBox();

        ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
        panel1.SuspendLayout();
        SuspendLayout();
        // 
        // button1
        // 
        backButton.Location = new Point(13, 12);
        backButton.Name = "button1";
        backButton.Size = new Size(98, 30);
        backButton.TabIndex = 0;
        backButton.Text = "button1";
        backButton.UseVisualStyleBackColor = true;
        // 
        // button2
        // 
        insertButton.Location = new Point(28, 422);
        insertButton.Name = "button2";
        insertButton.Size = new Size(98, 30);
        insertButton.TabIndex = 1;
        insertButton.Text = "button2";
        insertButton.UseVisualStyleBackColor = true;
        // 
        // button3
        // 
        updateButton.Location = new Point(425, 422);
        updateButton.Name = "button3";
        updateButton.Size = new Size(98, 30);
        updateButton.TabIndex = 2;
        updateButton.Text = "button3";
        updateButton.UseVisualStyleBackColor = true;
        // 
        // button4
        // 
        deleteButton.Location = new Point(604, 422);
        deleteButton.Name = "button4";
        deleteButton.Size = new Size(98, 30);
        deleteButton.TabIndex = 3;
        deleteButton.Text = "button4";
        deleteButton.UseVisualStyleBackColor = true;
        // 
        // button5
        // 
        loadButton.Location = new Point(234, 422);
        loadButton.Name = "button5";
        loadButton.Size = new Size(98, 30);
        loadButton.TabIndex = 4;
        loadButton.Text = "button5";
        loadButton.UseVisualStyleBackColor = true;
        // 
        // dataGridView1
        // 
        dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridView1.Location = new Point(13, 171);
        dataGridView1.Name = "dataGridView1";
        dataGridView1.RowHeadersWidth = 53;
        dataGridView1.Size = new Size(689, 171);
        dataGridView1.TabIndex = 5;
        // 
        // label1
        // 
        firstNameLabel.AutoSize = true;
        firstNameLabel.Location = new Point(50, 368);
        firstNameLabel.Name = "label1";
        firstNameLabel.Size = new Size(52, 21);
        firstNameLabel.TabIndex = 6;
        firstNameLabel.Text = "label1";
        // 
        // label2
        // 
        secondNameLabel.AutoSize = true;
        secondNameLabel.Location = new Point(253, 368);
        secondNameLabel.Name = "label2";
        secondNameLabel.Size = new Size(52, 21);
        secondNameLabel.TabIndex = 6;
        secondNameLabel.Text = "label1";
        // 
        // label3
        // 
        phoneLabel.AutoSize = true;
        phoneLabel.Location = new Point(443, 368);
        phoneLabel.Name = "label3";
        phoneLabel.Size = new Size(52, 21);
        phoneLabel.TabIndex = 6;
        phoneLabel.Text = "label1";
        // 
        // label4
        // 
        nationalIDLabel.AutoSize = true;
        nationalIDLabel.Location = new Point(623, 368);
        nationalIDLabel.Name = "label4";
        nationalIDLabel.Size = new Size(52, 21);
        nationalIDLabel.TabIndex = 6;
        nationalIDLabel.Text = "label1";
        nationalIDLabel.Click += label4_Click;
        // 
        // textBox1
        // 
        firstNameText.Location = new Point(188, 21);
        firstNameText.Name = "textBox1";
        firstNameText.Size = new Size(130, 29);
        firstNameText.TabIndex = 7;
        // 
        // textBox2
        // 
        lastNameText.Location = new Point(20, 21);
        lastNameText.Name = "textBox2";
        lastNameText.Size = new Size(130, 29);
        lastNameText.TabIndex = 7;
        // 
        // textBox3
        // 
        phoneText.Location = new Point(368, 21);
        phoneText.Name = "textBox3";
        phoneText.Size = new Size(130, 29);
        phoneText.TabIndex = 7;
        // 
        // textBox4
        // 
        nationalIdText.Location = new Point(537, 21);
        nationalIdText.Name = "textBox4";
        nationalIdText.Size = new Size(130, 29);
        nationalIdText.TabIndex = 7;
        // 
        // panel1
        // 
        panel1.Controls.Add(lastNameText);
        panel1.Controls.Add(phoneText);
        panel1.Controls.Add(nationalIdText);
        panel1.Controls.Add(firstNameText);
        panel1.Location = new Point(13, 61);
        panel1.Name = "panel1";
        panel1.Size = new Size(689, 82);
        panel1.TabIndex = 8;
        // 
        // GuestsForm
        // 
        ClientSize = new Size(892, 593);
        Controls.Add(panel1);
        Controls.Add(nationalIDLabel);
        Controls.Add(phoneLabel);
        Controls.Add(secondNameLabel);
        Controls.Add(firstNameLabel);
        Controls.Add(dataGridView1);
        Controls.Add(loadButton);
        Controls.Add(deleteButton);
        Controls.Add(updateButton);
        Controls.Add(insertButton);
        Controls.Add(backButton);
        Name = "GuestsForm";
        ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();

    }

    private void label4_Click(object sender, EventArgs e)
    {

    }

    private Button backButton;
    private Button insertButton;
    private Button updateButton;
    private Button deleteButton;
    private Button loadButton;
    private DataGridView dataGridView1;
    private Label firstNameLabel;
    private Label secondNameLabel;
    private Label phoneLabel;
    private Label nationalIDLabel;
    private TextBox firstNameText;
    private TextBox lastNameText;
    private TextBox phoneText;
    private TextBox nationalIdText;
    private Panel panel1;
}
