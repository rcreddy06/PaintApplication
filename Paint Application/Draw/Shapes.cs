// Ravichandra Reddy Chintalapelli
// Red ID# 816832621


using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace Draw
{
    public enum ShapeType { Line, Rectangle, FreeLine, Text, None };

    [Serializable]
    public abstract class Shape
    {
        public static bool MouseIsDown;	// Client program must set to true while drawing any shape using the mouse.
        // (Set in MouseIsDown handler when drawing; clear in MouseUp handler)
        // When initially displaying a file read from disk, MouseIsDown should be false.
        public static Shape CurrentShape;	// The shape objext that is currently being drawn with the mouse.
        // Client program must set in MouseIsDown handler when new shape is created
        public static Font CurrentFont = new Font("Courier New", 10);
        public static Encoding TextEncodingType = Encoding.ASCII;

        //public Pen Pen {get; set;}
        public Color PenColor { get; set; }
        public int PenWidth { get; set; }
        public Point Pt1 { get; set; }
        public Point Pt2 { get; set; }	// Not a member of the final shape for some figures but needed
        // for drawing temporary shapes (e.g., Text shape)- allows MouseIsDown
        //  handler in client program to save new point.
        public abstract void Draw(Graphics g);
        public abstract void writeBinary(BinaryWriter bw);
        public abstract void readBinary(BinaryReader br);
        public abstract void writeText(StreamWriter sw);
        public abstract void readText(StreamReader sr);
        public abstract XElement createXmlElement();
        public abstract void readXmlElement(XElement xe);


        public static Shape CreateShape(ShapeType type)
        {
            switch (type)
            {
                case ShapeType.Line:
                    return new Line();
                case ShapeType.Rectangle:
                    return new Rect();
                case ShapeType.FreeLine:
                    return new FreeLine();
                case ShapeType.Text:
                    return new Text();
                default:
                    return null;
            }
        }

        
        /////////////////// ADD UTILITY FUNCTIONS HERE IF DESIRED //////////////////////

    }  // End Shape class

    [Serializable]

    public class Line : Shape
    {

        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(PenColor, PenWidth);
            //MessageBox.Show(PenColor.ToString());
            g.DrawLine(pen, Pt1, Pt2);
        }

        public override string ToString()
        {
            string s = string.Format("Line: ({0},{1}), ({2},{3}), {4}, {5})",
                Pt1.X, Pt1.Y, Pt2.X, Pt2.Y, PenWidth, PenColor);
            return s;
        }

        public override void writeBinary(BinaryWriter bw)
        {
            // Writing binary/design criteria for Line


            bw.Write("Line");
            bw.Write(Pt1.X); bw.Write(Pt1.Y);
            bw.Write(Pt2.X); bw.Write(Pt2.Y);
            bw.Write(PenColor.ToArgb());
            bw.Write((int)PenWidth);

        }

        public override void readBinary(BinaryReader br)
        {
            //  Reading binary/design criteria for Line


            Pt1 = new Point(br.ReadInt32(), br.ReadInt32());
            Pt2 = new Point(br.ReadInt32(), br.ReadInt32());

            PenColor = Color.FromArgb(br.ReadInt32());
            PenWidth = br.ReadInt32();
            Shape.MouseIsDown = true;

        }

        public override void writeText(StreamWriter sw)
        {
            //Writing Text /design criteria for Line
            int alphaNumber = int.Parse((PenColor.A).ToString());
            int redNumber = int.Parse((PenColor.R).ToString());
            int greenNumber = int.Parse((PenColor.G).ToString());
            int blueNumber = int.Parse((PenColor.B).ToString());
            string hex = alphaNumber.ToString("x2") + redNumber.ToString("x2") + greenNumber.ToString("x2") + blueNumber.ToString("x2");

            sw.WriteLine(base.GetType().Name);
            sw.WriteLine(hex);
            sw.WriteLine(PenWidth);
            sw.WriteLine(Pt1);
            sw.WriteLine(Pt2);
        }

        public override void readText(StreamReader sr)
        {
            // Reading Text /design criteria for Line
            int iColorInt = (int.Parse(sr.ReadLine(), System.Globalization.NumberStyles.HexNumber));
            MessageBox.Show(iColorInt.ToString());
            PenColor = System.Drawing.Color.FromArgb(iColorInt);
            PenWidth = Convert.ToInt32(sr.ReadLine());
            //Pen shapePen = new Pen(PenColor, PenWidth);

            var g1 = Regex.Replace(sr.ReadLine(), @"[\{\}a-zA-Z=]", "").Split(',');
            Pt1 = new Point(int.Parse(g1[0]), int.Parse(g1[1]));
            var g2 = Regex.Replace(sr.ReadLine(), @"[\{\}a-zA-Z=]", "").Split(',');
            Pt2 = new Point(int.Parse(g2[0]), int.Parse(g2[1]));
        }
        public override XElement createXmlElement()
        {
            int alphaNumber = int.Parse((PenColor.A).ToString());
            int redNumber = int.Parse((PenColor.R).ToString());
            int greenNumber = int.Parse((PenColor.G).ToString());
            int blueNumber = int.Parse((PenColor.B).ToString());
            string hex = alphaNumber.ToString("x2") + redNumber.ToString("x2") + greenNumber.ToString("x2") + blueNumber.ToString("x2");

            XAttribute penAttribute1 = new XAttribute("Width", PenWidth);
            XAttribute penAttribute2 = new XAttribute("Color", hex);
            XElement pen = new XElement("Pen", penAttribute1, penAttribute2);
            // Writing points to XML
            XAttribute pointX = new XAttribute("X", Pt1.X);
            XAttribute pointY = new XAttribute("Y", Pt1.Y);
            XElement point1 = new XElement("Point1", pointX, pointY);
            pointX = new XAttribute("X", Pt2.X);
            pointY = new XAttribute("Y", Pt2.Y);
            XElement point2 = new XElement("Point2", pointX, pointY);
            XElement points = new XElement("Points", point1, point2);

            XElement lineElement = new XElement("Line", pen, points);
            return lineElement;
        }


        public override void readXmlElement(XElement xe)
        {
            if (xe.Name.ToString() == "Pen")//read pen properties
            {

                PenWidth = int.Parse(xe.Attribute("Width").Value);
                Int32 iColorInt = Convert.ToInt32(xe.Attribute("Color").Value, 16);
                PenColor = System.Drawing.Color.FromArgb(iColorInt);

            }
            else if (xe.Name.ToString() == "Points")
            {
                foreach (XElement xee in xe.Elements())
                {
                    if (xee.Name.ToString() == "Point1")
                    {
                        Pt1 = new Point(int.Parse((xee.Attribute("X").Value)), int.Parse((xee.Attribute("Y").Value)));

                        // Reading Location of the Point 1
                    }
                    if (xee.Name.ToString() == "Point2")
                    {
                        Pt2 = new Point(int.Parse((xee.Attribute("X").Value)), int.Parse((xee.Attribute("Y").Value)));

                        // Reading Location of the Point2
                    }
                }
            }
        }

    } // End line class

    [Serializable]

    public class Rect : Line
    {

        public override void Draw(Graphics g)
        {
            int x = Math.Min(Pt1.X, Pt2.X);
            int y = Math.Min(Pt1.Y, Pt2.Y);
            int width = Math.Abs(Pt2.X - Pt1.X);
            int height = Math.Abs(Pt2.Y - Pt1.Y);
            Rectangle rect = new Rectangle(x, y, width, height);
            Pen pen = new Pen(PenColor, PenWidth);
            g.DrawRectangle(pen, rect);
        }

        public override string ToString()
        {
            string s = string.Format("Rectangle: ({0},{1}), ({2},{3}), {4}, {5})",
                        Pt1.X, Pt1.Y, Pt2.X, Pt2.Y, (int)PenWidth, PenColor);
            return s;

        }

        public override void writeBinary(BinaryWriter bw)
        {
            // Writing binary/design criteria for Rectangle
            bw.Write("Rect");
            bw.Write(PenColor.ToArgb());
            bw.Write(PenWidth);
            bw.Write(Pt1.X); bw.Write(Pt1.Y);
            bw.Write(Pt2.X); bw.Write(Pt2.Y);
        }

        public override void readBinary(BinaryReader br)
        {
            // Reading binary/design criteria for Rectangle
            PenColor = Color.FromArgb(br.ReadInt32());
            PenWidth = br.ReadInt32();

            Pt1 = new Point(br.ReadInt32(), br.ReadInt32());
            Pt2 = new Point(br.ReadInt32(), br.ReadInt32());

            Shape.MouseIsDown = true;
        }

        public override void writeText(StreamWriter sw)
        {
            // Writing Text/design criteria for Rectangle
            int alphaNumber = int.Parse((PenColor.A).ToString());
            int redNumber = int.Parse((PenColor.R).ToString());
            int greenNumber = int.Parse((PenColor.G).ToString());
            int blueNumber = int.Parse((PenColor.B).ToString());
            string hex = alphaNumber.ToString("x2") + redNumber.ToString("x2") + greenNumber.ToString("x2") + blueNumber.ToString("x2");

            sw.WriteLine(base.GetType().Name);
            sw.WriteLine(hex);
            sw.WriteLine(PenWidth);
            sw.WriteLine(Pt1);
            sw.WriteLine(Pt2);

        }

        public override void readText(StreamReader sr)
        {
            //Reading Text /design criteria for Rectangle
            int iColorInt = (int.Parse(sr.ReadLine(), System.Globalization.NumberStyles.HexNumber));
            MessageBox.Show(iColorInt.ToString());
            PenColor = System.Drawing.Color.FromArgb(iColorInt);
            PenWidth = Convert.ToInt32(sr.ReadLine());

            var g1 = Regex.Replace(sr.ReadLine(), @"[\{\}a-zA-Z=]", "").Split(',');
            Pt1 = new Point(int.Parse(g1[0]), int.Parse(g1[1]));
            var g2 = Regex.Replace(sr.ReadLine(), @"[\{\}a-zA-Z=]", "").Split(',');
            Pt2 = new Point(int.Parse(g2[0]), int.Parse(g2[1]));
        }
        public override XElement createXmlElement()
        {
            int alphaNumber = int.Parse((PenColor.A).ToString());
            int redNumber = int.Parse((PenColor.R).ToString());
            int greenNumber = int.Parse((PenColor.G).ToString());
            int blueNumber = int.Parse((PenColor.B).ToString());
            string hex = alphaNumber.ToString("x2") + redNumber.ToString("x2") + greenNumber.ToString("x2") + blueNumber.ToString("x2");

            XAttribute penAttribute1 = new XAttribute("Width", PenWidth);
            XAttribute penAttribute2 = new XAttribute("Color", hex);
            XElement pen = new XElement("Pen", penAttribute1, penAttribute2);
            // Writing points to XML
            XAttribute pointX = new XAttribute("X", Pt1.X);
            XAttribute pointY = new XAttribute("Y", Pt1.Y);
            XElement point1 = new XElement("Point1", pointX, pointY);
            pointX = new XAttribute("X", Pt2.X);
            pointY = new XAttribute("Y", Pt2.Y);
            XElement point2 = new XElement("Point2", pointX, pointY);
            XElement points = new XElement("Points", point1, point2);

            XElement rectElement = new XElement("Rect", pen, points);
            return rectElement;
        }


    } // End Rect class

    [Serializable]

    public class FreeLine : Shape
    {
        public List<Point> FreeList { get; set; }


        public FreeLine()
            : base()
        {
            FreeList = new List<Point>();
            //FreeList.Add(Point.Empty);
            //FreeList.Add(Point.Empty);
        }

        public override void Draw(Graphics g)
        {
            // If this call to Draw is drawing the shape that is currently being drawn, then add
            // Pt2 to the list.  If the shape being drawn is a FreeLine already in the list,
            // do not add Pt2.

            if (Shape.MouseIsDown && this == Shape.CurrentShape)
                FreeList.Add(Pt2);

            // Client program must set Pt2 to the new point on MouseMove
            // while drawing a FreeLine (like it does for all shapes)

            if (FreeList.Count > 1)
            {
                Point[] ptArray = FreeList.ToArray();
                Pen pen = new Pen(PenColor, PenWidth);
                g.DrawLines(pen, ptArray);
            }           

        }


        public override string ToString()
        {
            string s = string.Format("FreeLine: ({0},{1})", (int)PenWidth, PenColor);
            foreach (Point p in FreeList)
                s += string.Format("({0},{1}) ", p.X, p.Y);
            return s;
        }


        public override void writeBinary(BinaryWriter bw)
        {
            // Writing binary/design criteria for Free Line
            bw.Write("FreeLine");
            bw.Write(PenColor.ToArgb());
            bw.Write(PenWidth);
            bw.Write(FreeList.Count);
            foreach (Point pointElement in FreeList)
            {
                bw.Write(pointElement.X); bw.Write(pointElement.Y);
            }
        }

        public override void readBinary(BinaryReader br)
        {
            // Reading binary/design criteria for Free Line
            Point pt = new Point();
            Int32 listCount = Convert.ToInt32(br.ReadInt32());
            for (int counter = 0; counter < listCount; counter++)
            {
                pt.X = br.ReadInt32(); pt.Y = br.ReadInt32();
                FreeList.Add(pt);
            }
        }

        public override void writeText(StreamWriter sw)
        {
            // Writing Text/design criteria for Free Line
            int alphaNumber = int.Parse((PenColor.A).ToString());
            int redNumber = int.Parse((PenColor.R).ToString());
            int greenNumber = int.Parse((PenColor.G).ToString());
            int blueNumber = int.Parse((PenColor.B).ToString());
            string hex = alphaNumber.ToString("x2") + redNumber.ToString("x2") + greenNumber.ToString("x2") + blueNumber.ToString("x2");

            sw.WriteLine(base.GetType().Name);
            sw.WriteLine(hex);
            sw.WriteLine(PenWidth);
            sw.WriteLine(FreeList.Count);
            foreach (Point pointElement in FreeList)
                sw.WriteLine(pointElement);
        }

        public override void readText(StreamReader sr)
        {
            // Reading Text/design criteria for Free Line

            int iColorInt = (int.Parse(sr.ReadLine(), System.Globalization.NumberStyles.HexNumber));
            MessageBox.Show(iColorInt.ToString());
            PenColor = System.Drawing.Color.FromArgb(iColorInt);
            PenWidth = Convert.ToInt32(sr.ReadLine());

            Int32 listCount = Convert.ToInt32(sr.ReadLine());
            for (int counter = 0; counter < listCount; counter++)
            {
                var g = Regex.Replace(sr.ReadLine(), @"[\{\}a-zA-Z=]", "").Split(',');
                FreeList.Add(new Point(int.Parse(g[0]), int.Parse(g[1])));
            }

        }
        public override XElement createXmlElement()
        {
            int alphaNumber = int.Parse((PenColor.A).ToString());
            int redNumber = int.Parse((PenColor.R).ToString());
            int greenNumber = int.Parse((PenColor.G).ToString());
            int blueNumber = int.Parse((PenColor.B).ToString());
            string hex = alphaNumber.ToString("x2") + redNumber.ToString("x2") + greenNumber.ToString("x2") + blueNumber.ToString("x2");

            XAttribute penAttribute1 = new XAttribute("Width", PenWidth);
            XAttribute penAttribute2 = new XAttribute("Color", hex);
            XElement pen = new XElement("Pen", penAttribute1, penAttribute2);

            List<XElement> points = new List<XElement>();
            XElement point;
            foreach (Point pointElement in FreeList)
            {
                point = new XElement("Point", new XAttribute("X", pointElement.X), new XAttribute("Y", pointElement.Y));
                points.Add(point);
            }
            XElement pts = new XElement("Points", points);
            XElement freeLineElement = new XElement("FreeLine", pen, pts);
            return freeLineElement;
        }
        public override void readXmlElement(XElement xe)
        {
            if (xe.Name.ToString() == "Pen")//read pen properties
            {

                PenWidth = int.Parse(xe.Attribute("Width").Value);
                Int32 iColorInt = Convert.ToInt32(xe.Attribute("Color").Value, 16);
                PenColor = System.Drawing.Color.FromArgb(iColorInt);

            }
            else if (xe.Name.ToString() == "Points")
            {


                foreach (XElement xee in xe.Elements())
                    FreeList.Add(new Point(int.Parse(xee.Attribute("X").Value), int.Parse(xee.Attribute("Y").Value)));
            }
        }


    } // End FreeLine class


    [Serializable]

    public class Text : Shape
    {
        /* Intended use for this shape type:
         * This Text type is intended for entering one line of text.
         * Text is entered at the point Pt1 (usually, where the mouse is clicked)
         * When Enter is pressed, no more text is accepted.
         * Backspace can be used to delete one character at a time before Enter is pressed.
         */

        public string TextLine { get; set; }
        public Font TextFont { get; set; }
        //public Brush TextBrush { get; set; }
        public bool Open { get; set; }	// keyboard input accepted iff Open = true


        public Text()
            : base()
        {
            //TextBrush = new SolidBrush(Color.Black);
            TextLine = "";
            TextFont = CurrentFont;
            Open = true;
        }

        public override void Draw(Graphics g)
        {
            Brush brush = new SolidBrush(PenColor);
            g.DrawString(TextLine, TextFont, brush, Pt1);
        }

        public override string ToString()
        {
            string s = string.Format("Text: ({0},{1}), {2}, {3}, {4})",
                Pt1.X, Pt1.Y, TextLine, TextFont.FontFamily, PenColor);
            return s;
        }

        public override void writeBinary(BinaryWriter bw)
        {

            // Writing binary/design criteria for Text
            bw.Write("Text");
            bw.Write(Pt1.X + "," + Pt1.Y + "," + TextLine + "," + TextFont.FontFamily.Name + "," + TextFont.SizeInPoints + "," + PenColor.ToArgb());
        }

        public override void readBinary(BinaryReader br)
        {
            // Reading binary/design criteria for Text
            string str = br.ReadString();
            string[] data = str.Split(',');
            this.Pt1 = new Point(int.Parse(data[0]), int.Parse(data[1]));
            this.TextLine = data[2];
            this.TextFont = new Font(data[3], float.Parse(data[4]));
            this.PenColor = Color.FromArgb(int.Parse(data[5]));
            Shape.MouseIsDown = true;
        }

        public override void writeText(StreamWriter sw)
        {
            // Writing Text/design criteria for Text
            sw.WriteLine("Text");
            sw.WriteLine(Pt1.X);
            sw.WriteLine(Pt1.Y);
            sw.WriteLine(TextLine);
            sw.WriteLine(TextFont.FontFamily.Name);
            sw.WriteLine(TextFont.SizeInPoints);
            sw.WriteLine(PenColor.Name);

        }

        public override void readText(StreamReader sr)
        {
            //Read Text/design criteria for Text
            Pt1 = new Point(int.Parse(sr.ReadLine()), int.Parse(sr.ReadLine()));
            TextLine = sr.ReadLine();
            TextFont = new Font(sr.ReadLine(), float.Parse(sr.ReadLine()));
            PenColor = Color.FromArgb(int.Parse(sr.ReadLine(), System.Globalization.NumberStyles.HexNumber));
            Shape.MouseIsDown = true;

        }

        public override XElement createXmlElement()
        {

            int alphaNumber = int.Parse((PenColor.A).ToString());
            int redNumber = int.Parse((PenColor.R).ToString());
            int greenNumber = int.Parse((PenColor.G).ToString());
            int blueNumber = int.Parse((PenColor.B).ToString());
            string hex = alphaNumber.ToString("x2") + redNumber.ToString("x2") + greenNumber.ToString("x2") + blueNumber.ToString("x2");

            XAttribute penAttribute1 = new XAttribute("Text", TextLine);
            XAttribute penAttribute2 = new XAttribute("Font", TextFont.FontFamily.Name);
            XAttribute penAttribute3 = new XAttribute("Size", TextFont.SizeInPoints);
            XAttribute penAttribute4 = new XAttribute("Color", hex);

            XElement pen = new XElement("Pen", penAttribute1, penAttribute2, penAttribute3,penAttribute4);
            // Writing points to XML File
            XAttribute pointX = new XAttribute("X", Pt1.X);
            XAttribute pointY = new XAttribute("Y", Pt1.Y);
            XElement point1 = new XElement("Point1", pointX, pointY);
            XElement points = new XElement("Points", point1);

            XElement textElement = new XElement("Text", pen, points);
            return textElement;
        }

        public override void readXmlElement(XElement xe)
        { 
             if (xe.Name.ToString() == "Pen")//read pen properties
            {
                TextLine = xe.Attribute("Text").Value;
                TextFont = new Font(xe.Attribute("Font").Value, float.Parse(xe.Attribute("Size").Value));
                Int32 iColorInt = Convert.ToInt32(xe.Attribute("Color").Value, 16);
                PenColor = System.Drawing.Color.FromArgb(iColorInt);

            }
             else if (xe.Name.ToString() == "Points")
             {
                 foreach (XElement xee in xe.Elements())
                 {
                     if (xee.Name.ToString() == "Point1")
                     {
                         Pt1 = new Point(int.Parse((xee.Attribute("X").Value)), int.Parse((xee.Attribute("Y").Value)));// Reading Location of the text...

                         }
                 }

             }
        }

    } // End Text class

}