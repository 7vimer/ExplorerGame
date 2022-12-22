using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;

namespace Bomber
{
	public partial class MainWindow : Window
	{
		DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);
		static List<Bomb> bombList = new List<Bomb>();
		List<Bomb> deleter = new List<Bomb>();
		static Image allText = new Image(); //24x24
		static Map map = new Map();
		static Canvas Field;
		static Player p;
		public MainWindow()
		{
			InitializeComponent();
			Game.Focus();
			Field = Game;
			
			timer.Interval = TimeSpan.FromMilliseconds(110);
			timer.Tick += update;
			timer.Start();
			allText.Source = new BitmapImage(new Uri("pack://application:,,,/allTextures.png"));
			map.loadMap();
			map.showMap();
			p = new Player();
		}

		private void update(object sender, EventArgs e)
		{
			foreach(Bomb bomb in bombList)
            {
				bomb.updateSkin();
				if(bomb.skin == 23)
                {
					deleter.Add(bomb);
					map.removeCell(bomb.masposY, bomb.masposX - 1);
					map.removeCell(bomb.masposY, bomb.masposX + 1);
					map.removeCell(bomb.masposY - 1, bomb.masposX);
					map.removeCell(bomb.masposY + 1, bomb.masposX);
				}
            }
			foreach(Bomb bomb in deleter)
            {
				bombList.Remove(bomb);
				Field.Children.Remove(bomb.recta);
            }
		}
		class Player
        {
			public int posx, posy, masposX, masposY;
			ImageBrush ib = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/player.png")));
			public Rectangle recta;
			public Player()
            {
				masposX = 1;
				masposY = 1;
				this.recta = new Rectangle
				{
					Height = 24,
					Width = 24,
					Fill = ib
				};
				posx = posy = 24;
				Canvas.SetTop(recta, posy);
				Canvas.SetLeft(recta, posx);
                Field.Children.Add(recta);
            }

			public void left()
            {
				if(map.mapMas[masposY, masposX -1] == 0)
                {
					posx -= 24;
					masposX -= 1;
					Canvas.SetTop(recta, posy);
					Canvas.SetLeft(recta, posx);
				}
            }
			public void right()
			{
				if (map.mapMas[masposY, masposX + 1] == 0)
				{
					posx += 24;
					masposX += 1;
					Canvas.SetTop(recta, posy);
					Canvas.SetLeft(recta, posx);
				}
			}
			public void up()
			{
				if (map.mapMas[masposY - 1, masposX] == 0)
				{
					posy -= 24;
					masposY -= 1;
					Canvas.SetTop(recta, posy);
					Canvas.SetLeft(recta, posx);
				}
			}
			public void down()
			{
				if (map.mapMas[masposY + 1, masposX] == 0)
				{
					posy += 24;
					masposY += 1;
					Canvas.SetTop(recta, posy);
					Canvas.SetLeft(recta, posx);
				}
			}
			public void placeBomb()
            {
				Bomb b = new Bomb();
				bombList.Add(b);
				Canvas.SetLeft(b.recta, posx);
				Canvas.SetTop(b.recta, posy);
				Field.Children.Add(b.recta);
            }
		}

		class Bomb
        {
			public int posx, posy, masposX, masposY;
			public int skin = 1;
			ImageBrush ib;
			public Rectangle recta;
			public Bomb()
            {
				posx = p.posx;
				posy = p.posy;
				masposX = p.masposX;
				masposY = p.masposY;

				recta = new Rectangle
				{
					Height = 24,
					Width = 24
				};
			}
			public void updateSkin()
            {
				ib = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/explosion/" + skin + ".png")));
				recta.Fill = ib;
				skin++;
			}
		}


		class Map
		{
			public int[,] mapMas;
			public void loadMap()
			{
				StreamReader sr = new StreamReader(Environment.CurrentDirectory + "/map.txt");

				List<int[]> zn = new List<int[]>();
				while(!sr.EndOfStream)
				{
					string dan = sr.ReadLine();
					string[] str = new string[dan.Length / 2 - 1];
					str = dan.Split(' ');
					int c = 0;
					int[] strint = new int[str.Length];
					foreach (var x in str)
					{
						strint[c] = Convert.ToInt32(x);
						c++;
					}
					zn.Add(strint);
				}
				mapMas = new int[zn.Count, zn[0].Length];
				for(int i = 0; i < zn.Count; i++)
                {
					for(int j = 0; j < zn[i].Length; j++)
                    {
						mapMas[i,j] = zn[i][j];
                    }
                }
				sr.Close();
			}
			public void showMap()
			{
				int posx = 0;
				int posy = 0;
				for (int i = 0; i < mapMas.GetLength(0); i++)
                {
					for (int j = 0; j < mapMas.GetLength(1); j++)
					{
						int id = 0;
						if (mapMas[i, j] != -1)
						{
							Rectangle rect = new Rectangle
							{
								Tag = "map",
								Height = 24,
								Width = 24,
								Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/" + mapMas[i, j] + ".png")))
							};
							Canvas.SetTop(rect, posy);
							Canvas.SetLeft(rect, posx);
							Field.Children.Add(rect);
						}
                        else
                        {
							Rectangle rect = new Rectangle
							{
								Tag = "map",
								Height = 24,
								Width = 24
							};
							Canvas.SetTop(rect, posy);
							Canvas.SetLeft(rect, posx);
							Field.Children.Add(rect);
						}
						posx += 24;
					}
					posx = 0;
					posy += 24;
				}
            }

			public void removeCell(int y, int x)
            {
                if (mapMas[y,x] == 2)
                {
					mapMas[y,x] = 0;
					int Count = 0;
					foreach(var c in Field.Children.OfType<Rectangle>())
                    {
						if((string)c.Tag == "map")
                        {
							Count++;
							if(Count == y * mapMas.GetLength(1) + x)
                            {
								c.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/0.png")));
							}
                        }
                    }
                }
            }
		}

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
			if(e.Key == Key.W)
            {
				p.up();
            }
			if (e.Key == Key.D)
			{
				p.right();
			}
			if (e.Key == Key.A)
			{
				p.left();
			}
			if (e.Key == Key.S)
			{
				p.down();
			}
			if(e.Key == Key.Space)
            {
				p.placeBomb();
            }
		}
    }
}
