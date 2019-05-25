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
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace FirstProject
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();
        bool humanCaptured = false;
        public MainWindow()
        {
            InitializeComponent();

            enemyTimer.Tick += EnemyTimer_Tick;
            enemyTimer.Interval = TimeSpan.FromSeconds(2);

            targetTimer.Tick += TargetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(.1);
        }

        private void TargetTimer_Tick(object sender, EventArgs e)
        {
            progressBar.Value += 1;
            if(progressBar.Value >= progressBar.Maximum)
            {
                EndTheGame();
            }
        }

        

        private void EndTheGame()
        {
            if (!PlayArea.Children.Contains(gameOverText))
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                humanCaptured = false;
                startButton.Visibility = Visibility.Visible;
                PlayArea.Children.Add(gameOverText);
            }
        }

        private void EnemyTimer_Tick(object sender, EventArgs e)
        {
            AddEnemy();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            human.IsHitTestVisible = true;
            humanCaptured = false;
            progressBar.Value = 0;
            startButton.Visibility = Visibility.Collapsed;
            PlayArea.Children.Clear();
            PlayArea.Children.Add(target);
            PlayArea.Children.Add(human);
            enemyTimer.Start();
            targetTimer.Start();
            Canvas.SetLeft(human, 0);
            Canvas.SetTop(human, 0);
            AddEnemy();
            AddEnemy();
            AddEnemy();
            AddEnemy();
        }

        private void AddEnemy()
        {
            ContentControl enemy = new ContentControl();
            enemy.Template = Resources["EnemyTemplate"] as ControlTemplate;
            AnimateEnemy(enemy, 0, PlayArea.ActualWidth - 50, "(Canvas.Left)");
            AnimateEnemy(enemy, random.Next((int)PlayArea.ActualHeight - 50), random.Next((int)PlayArea.ActualHeight - 50), "(Canvas.Top)");
            PlayArea.Children.Add(enemy);
            enemy.MouseEnter += enemy_MouseEnter;
        }

        private void enemy_MouseEnter(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                EndTheGame();
            }
        }

        private void AnimateEnemy(ContentControl enemy, double from, double to, string propertyToAnimate)
        {
            Storyboard storyBoard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 6)))
            };
            Storyboard.SetTarget(animation, enemy);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
            storyBoard.Children.Add(animation);
            storyBoard.Begin();
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Human_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (enemyTimer.IsEnabled)
            {
                humanCaptured = true;
                human.IsHitTestVisible = false;
            }
        }

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            if (targetTimer.IsEnabled && humanCaptured)
            {
                progressBar.Value = 0;
                Canvas.SetLeft(target, random.Next(100, (int)PlayArea.ActualWidth- 100));
                Canvas.SetTop(target, random.Next(100, (int)PlayArea.ActualHeight - 100));
                Canvas.SetLeft(human, random.Next(100, (int)PlayArea.ActualWidth - 100));
                Canvas.SetTop(human, random.Next(100, (int)PlayArea.ActualHeight - 100));
                humanCaptured = false;
                human.IsHitTestVisible = true;
            }
        }

        private void PlayArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                Point pointerPosition = e.GetPosition(null);
                Point relativePosition = grid.TransformToVisual(PlayArea).Transform(pointerPosition);
                if((Math.Abs(relativePosition.X - Canvas.GetLeft(human)) > human.ActualWidth * 3)
                    || (Math.Abs(relativePosition.Y - Canvas.GetTop(human)) > human.ActualHeight * 3))
                {
                    humanCaptured = false;
                    human.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(human, relativePosition.X - human.ActualWidth / 2);
                    Canvas.SetTop(human, relativePosition.Y - human.ActualHeight / 2);
                }
            }
        }

        private void PlayArea_MouseLeave(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                EndTheGame();
            }
        }

        private void Target_MouseEnter(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                EndTheGame();
            }
        }
    }
}
