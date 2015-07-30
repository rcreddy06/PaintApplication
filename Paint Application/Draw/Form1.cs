// Ravichandra Reddy Chintalapelli
// Red ID# 816832621


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;


namespace Draw
{
    public partial class Form1 : Form
    {
        Point pt1;
        Color penColor = Color.FromArgb(255, 0, 0);
        int penWidth = 2;
        List<Shape> shapeList = new List<Shape>();
        ShapeType currentShapeType = ShapeType.Line;
        Shape shape;
        bool dataModified = false;
        string currentFile; // = "drawing1.bin


        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentShapeType == ShapeType.None)
            {
                MessageBox.Show("Must select shape type");
                return;
            }
            pt1 = e.Location;
            Console.WriteLine("currentShapeType = {0}", currentShapeType);
            shape = Shape.CreateShape(currentShapeType);
            shape.Pt1 = pt1;
            shape.PenColor = Color.Black;
            //shape.Pen = penTemp;
            Shape.MouseIsDown = true;
            Shape.CurrentShape = shape;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Shape.MouseIsDown && e.Button == MouseButtons.Left)
            {
                shape.Pt2 = e.Location;
                Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;	// Don't respond to right mouse button up

            //Graphics g = this.CreateGraphics();
            shape.Pt2 = e.Location;
            shape.PenColor = penColor;
            shape.PenWidth = penWidth;
            //penFinal = new Pen(penColor, penWidth);
            shapeList.Add(shape);
            Shape.MouseIsDown = false;
            Invalidate();
            dataModified = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Draw shapes already in shapeList
            foreach (Shape s in shapeList)
                s.Draw(e.Graphics);

            // Draw current shape, if any
            if (Shape.MouseIsDown && shape != null)
                shape.Draw(e.Graphics);
        }

        private void penWidthMenuItem_Click(object sender, EventArgs e)
        {
            PenDialog dlg = new PenDialog();
            dlg.PenWidth = penWidth;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                penWidth = dlg.PenWidth;
            }
        }

        private void lineMenuItem_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Line;
            clearShapeChecks();
            lineToolStripMenuItem.Checked = true;
        }

        private void rectangleMenuItem_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Rectangle;
            clearShapeChecks();
            rectangleToolStripMenuItem.Checked = true;
        }

        private void freeLineMenuItem_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.FreeLine;
            clearShapeChecks();
            freeLineToolStripMenuItem.Checked = true;
        }

        private void textMenuItem_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Text;
            FontDialog dlg = new FontDialog();
            DialogResult dr = dlg.ShowDialog();
            if (dr == DialogResult.OK)
                Shape.CurrentFont = dlg.Font;

            clearShapeChecks();
            textToolStripMenuItem.Checked = true;
        }

        private void printShapesMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("\nAll Shapes");
            foreach (Shape shape in shapeList)
                Console.WriteLine(shape);
        }

        private void penColorMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = penColor;
            if (dlg.ShowDialog() == DialogResult.OK)
                penColor = dlg.Color;

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentShapeType == ShapeType.Text)
            {
                Text t = (Text)shape;
                if (e.KeyCode == Keys.Enter)
                    t.Open = false;
                if (e.KeyCode == Keys.Back && t.Open)
                {
                    t.TextLine = t.TextLine.Substring(0, t.TextLine.Length - 1);
                    Invalidate();
                    Update();
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (currentShapeType == ShapeType.Text)
            {
                if (((Text)shape).Open == false)
                    return;
                if (e.KeyChar == '\b')
                    return;

                ((Text)shape).TextLine += e.KeyChar;
                Graphics g = this.CreateGraphics();
                shape.Draw(g);
            }
        }
        private void uTF8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shape.TextEncodingType = Encoding.UTF8;
            clearEncodingChecks();
            uTF8ToolStripMenuItem.Checked = true;
        }

        private void uTF16ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shape.TextEncodingType = Encoding.Unicode;
            clearEncodingChecks();
            uTF16ToolStripMenuItem.Checked = true;
        }

        private void uTF32ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shape.TextEncodingType = Encoding.UTF32;
            clearEncodingChecks();
            uTF32ToolStripMenuItem.Checked = true;
        }

        private void aSCIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shape.TextEncodingType = Encoding.ASCII;
            clearEncodingChecks();
            aSCIIToolStripMenuItem.Checked = true;
        }

        private void bigEndianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shape.TextEncodingType = Encoding.BigEndianUnicode;
            clearEncodingChecks();
            bigEndianToolStripMenuItem.Checked = true;
        }

        private void uTF7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shape.TextEncodingType = Encoding.UTF7;
            clearEncodingChecks();
            uTF7ToolStripMenuItem.Checked = true;
        }

        public void clearEncodingChecks()
        {
            aSCIIToolStripMenuItem.Checked = false;
            uTF8ToolStripMenuItem.Checked = false;
            uTF16ToolStripMenuItem.Checked = false;
            uTF32ToolStripMenuItem.Checked = false;
            uTF7ToolStripMenuItem.Checked = false;
            bigEndianToolStripMenuItem.Checked = false;
        }

        public void clearShapeChecks()
        {
            lineToolStripMenuItem.Checked = false;
            rectangleToolStripMenuItem.Checked = false;
            freeLineToolStripMenuItem.Checked = false;
            textToolStripMenuItem.Checked = false;
        }

        private void saveAsMenuItem_Click(object sender, EventArgs e)
        {
            this.saveDrawing();
            reset();
        }

        private void openMenuItem_Click(object sender, EventArgs e)
        {
            //Open Menu & check condition
            if (dataModified)
            {
                // Offer to save drawing & display dialog
                DialogResult dialogResult = MessageBox.Show("Do you want to save changes?", "Form1", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dialogResult)
                {
                    case DialogResult.Yes: writeFile(currentFile);
                        break;
                    case DialogResult.No: reset();
                        this.CreateGraphics().Clear(Form1.ActiveForm.BackColor);
                        break;
                    case DialogResult.Cancel: return;
                }
            }

            //open dialog
            String line = null;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files(*.txt)|*.txt|Binary Files(*.bin)|*.bin|Serialized files(*.ser)|*.ser|XML files(*.xml)|*.xml|All files(*.*)|*.*";
            dlg.InitialDirectory = Directory.GetCurrentDirectory();

            if (dlg.ShowDialog() == DialogResult.Cancel)
                return;

            reset();
            this.CreateGraphics().Clear(Form1.ActiveForm.BackColor);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                currentFile = dlg.FileName;
                //FileStream fs = new FileStream(currentFile, FileMode.Open);
                FileInfo fileInfo = new FileInfo(dlg.FileName);
                Graphics graphics = this.CreateGraphics();

                // check for text files & selecting the shape
                if (fileInfo.Extension == ".txt")
                {
                    FileStream fs = new FileStream(currentFile, FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    while ((line = sr.ReadLine()) != null)
                    {
                        switch (line)
                        {
                            case "Line": currentShapeType = ShapeType.Line;
                                break;
                            case "Rect": currentShapeType = ShapeType.Rectangle;
                                break;
                            case "FreeLine": currentShapeType = ShapeType.FreeLine;
                                break;
                            case "Text": currentShapeType = ShapeType.Text;
                                break;
                        }
                        shape = Shape.CreateShape(currentShapeType);

                        shape.readText(sr);
                        shape.Draw(graphics);
                        shapeList.Add(shape);
                    }
                    sr.Close();
                    fs.Close();
                }
                // Check for bin files & selecting encoding 
                else if (fileInfo.Extension == ".bin")
                {
                    FileStream fs = new FileStream(currentFile, FileMode.Open);
                    BinaryReader br = new BinaryReader(fs);

                    switch (br.ReadString())
                    {
                        case "us-ascii":
                            Shape.TextEncodingType = Encoding.ASCII;
                            break;
                        case "utf-8":
                            Shape.TextEncodingType = Encoding.UTF8;
                            break;
                        case "u\0t\0f\0-\01\06\0":
                            Shape.TextEncodingType = Encoding.Unicode;
                            break;
                        case "u\0\0\0t\0\0\0f\0\0\0-\0\0\03\0\0\02\0\0\0":
                            Shape.TextEncodingType = Encoding.UTF32;
                            break;
                        case "\0u\0n\0i\0c\0o\0d\0e\0F\0F\0F\0E":
                            Shape.TextEncodingType = Encoding.BigEndianUnicode;
                            break;
                        case "utf-7":
                            Shape.TextEncodingType = Encoding.UTF7;
                            break;
                    }

                    br = new BinaryReader(fs, Shape.TextEncodingType);

                    int length = (int)br.BaseStream.Length;
                    MessageBox.Show(length.ToString());
                    while (br.PeekChar() != -1)
                    {
                        // Selecting shape for bin file & recreating the figures
                        switch (br.ReadString())
                        {
                            case "Line":
                                currentShapeType = ShapeType.Line;
                                shape = Shape.CreateShape(currentShapeType);
                                shape.readBinary(br);
                                shape.Draw(graphics);
                                shapeList.Add(shape);
                                break;

                            case "Rect":
                                currentShapeType = ShapeType.Rectangle;
                                shape = Shape.CreateShape(currentShapeType);
                                shape.readBinary(br);
                                shape.Draw(graphics);
                                shapeList.Add(shape);
                                break;

                            case "FreeLine":
                                currentShapeType = ShapeType.FreeLine;
                                shape = Shape.CreateShape(currentShapeType);
                                shape.readBinary(br);
                                shape.Draw(graphics);
                                shapeList.Add(shape);
                                break;

                            case "Text":
                                currentShapeType = ShapeType.Text;
                                shape = Shape.CreateShape(currentShapeType);
                                shape.readBinary(br);
                                shape.Draw(graphics);
                                shapeList.Add(shape);
                                break;
                        }
                    }
                    fs.Close();
                }
                else if (fileInfo.Extension == ".ser")
                {

                    FileStream fs = new FileStream(currentFile, FileMode.Open);
                    BinaryFormatter bf = new BinaryFormatter();
                    shapeList = (List<Shape>)bf.Deserialize(fs);

                    foreach (Shape shape in shapeList)

                        shape.Draw(graphics);
                    fs.Close();
                }//end ser
                else if (fileInfo.Extension == ".xml")
                {

                    XDocument document = XDocument.Load(currentFile);
                    XElement root = document.Root;
                    foreach (XElement xe in root.Elements())
                    {
                        switch (xe.Name.ToString())
                        {
                            case "Line": currentShapeType = ShapeType.Line;
                                break;
                            case "Rect": currentShapeType = ShapeType.Rectangle;
                                break;
                            case "FreeLine": currentShapeType = ShapeType.FreeLine;
                                break;
                            case "Text": currentShapeType = ShapeType.Text;
                                break;
                        }
                        shape = Shape.CreateShape(currentShapeType);
                        foreach (XElement xee in xe.Elements())
                        {
                            shape.readXmlElement(xee);
                        }

                        shape.Draw(graphics);

                        shapeList.Add(shape);
                        //fs1.Close();
                    }// End  xml
                }
            }
        }

        private void newMenuItem_Click(object sender, EventArgs e)
        {
            // New Menu & reset if needed 

            if (dataModified)
            {
                // Offer to save drawing
                DialogResult dialogResult = MessageBox.Show("Do you want to save changes?", "Form1", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dialogResult)
                {
                    case DialogResult.Yes: writeFile(currentFile);
                        break;
                    case DialogResult.No: reset();
                        this.CreateGraphics().Clear(Form1.ActiveForm.BackColor);
                        break;
                    case DialogResult.Cancel: return;
                }
            }
            reset();
            this.CreateGraphics().Clear(Form1.ActiveForm.BackColor);
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            // Save file

            if (currentFile == null)
                saveDrawing();
            else
                writeFile(currentFile);
            dataModified = false;
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            // check for modifications and Exit 
            if (dataModified)
            {
                // Offer to save drawing
                DialogResult dialogResult = MessageBox.Show("Do you want to save changes?", "Form1", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dialogResult)
                {
                    case DialogResult.Yes: writeFile(currentFile);
                        break;
                    case DialogResult.No: reset();
                        this.CreateGraphics().Clear(Form1.ActiveForm.BackColor);
                        break;
                    case DialogResult.Cancel: return;
                }
            }
            Application.Exit();
        }



        public void saveDrawing()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Form1";
            dlg.Filter = "Text files(*.txt)|*.txt|Binary Files(*.bin)|*.bin|Serialized files(*.ser)|*.ser|XML files(*.xml)|*.xml|All files(*.*)|*.*";
            dlg.InitialDirectory = Directory.GetCurrentDirectory();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                String fileName = dlg.FileName;
                this.writeFile(fileName);
            }
        }

        public void writeFile(String fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            if (fileInfo.Extension == ".txt")
            {
                FileStream fs = new FileStream(fileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, Shape.TextEncodingType);
                shapeList.ForEach(delegate(Shape s)
                { s.writeText(sw); });
                sw.Close();
                fs.Close();
            }
            else if (fileInfo.Extension == ".bin")
            {
                FileStream fs = new FileStream(fileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs, Shape.TextEncodingType);
                bw.Write(Shape.TextEncodingType.BodyName);

                shapeList.ForEach(delegate(Shape s)
                { s.writeBinary(bw); });
                bw.Close();
                fs.Close();

            }
         
             else if(fileInfo.Extension == ".ser")
            {
                FileStream fs = new FileStream(fileName, FileMode.Create);

                BinaryFormatter bf = new BinaryFormatter();
                try
                {
                    bf.Serialize(fs, shapeList);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                fs.Close();
            }
            else if(fileInfo.Extension == ".xml")
            {
                XDocument document = new XDocument();
                XElement root = new XElement("drawing");
                foreach (Shape shape in shapeList)
                    root.Add(shape.createXmlElement());
                document.Add(root);
                document.Save(fileName);
            }
           currentFile = fileName;
        
        }


        public void reset()
        {
            //reset the form roperties
            penColor = Color.FromArgb(255, 255, 0, 0);
            penWidth = 10;
            shapeList.Clear();
            currentShapeType = ShapeType.Line;
            dataModified = false;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }  // end class Form1   

}