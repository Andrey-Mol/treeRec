using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FractalTree.Main
{
    public partial class MainWindow : Window
    {
        private Random random = new Random();

        // Параметры для настройки
        private int recursionDepth = 6; // Глубина рекурсии
        private double branchProbability = 0.8; // Вероятность создания ветки
        private double baseAngle = 30; // Базовый угол ветвления
        private int branchesPerNode = 3; // Количество ветвей на каждом узле
        private double trunkLength = 150; // Длина начального ствола
        private double lengthReductionFactor = 0.7; // Фактор уменьшения длины веток
        private int branchDensityIncrease = 2; // Увеличение плотности веток на каждом уровне
        private double branchingPointThreshold = 0.5; // Минимальная доля длины ветки для генерации новых веток

        public MainWindow()
        {
            InitializeComponent();

            // Отрисовка первой пальмы
            DrawPalmTree();
        }

        private void DrawPalmTree()
        {
            // Очистка канвы
            MainCanvas.Children.Clear();

            // Начальные координаты пальмы
            double startX = MainCanvas.ActualWidth / 2;
            double startY = MainCanvas.ActualHeight - 50;

            // Запускаем рекурсивное рисование пальмы
            DrawPalmBranch(startX, startY, -90, trunkLength, 0, recursionDepth);
        }

        private void DrawPalmBranch(double x, double y, double angle, double length, int level, int maxLevel)
        {
            if (level > maxLevel || length < 5)
                return;

            // Конечная точка текущей ветки
            double x2 = x + Math.Cos(angle * Math.PI / 180) * length;
            double y2 = y + Math.Sin(angle * Math.PI / 180) * length;

            // Устанавливаем цвет и толщину ветки
            Brush color = level < 2 ? Brushes.SandyBrown : Brushes.DarkGreen;
            double thickness = level < 2 ? maxLevel - level + 1 : 1;

            Line branch = new Line
            {
                X1 = x,
                Y1 = y,
                X2 = x2,
                Y2 = y2,
                Stroke = color,
                StrokeThickness = thickness
            };

            MainCanvas.Children.Add(branch);

            // Если вероятность создания ветки не срабатывает, ветвь не создается
            if (random.NextDouble() > branchProbability)
                return;

            // Увеличиваем количество веток на каждом уровне
            int currentBranchesPerNode = branchesPerNode + level * branchDensityIncrease;

            // Рисуем несколько ветвей из разных точек вдоль текущей ветки
            for (int i = 0; i < currentBranchesPerNode; i++)
            {
                // Случайная точка вдоль текущей ветки
                double branchingPoint = random.NextDouble(); // От 0 (начало) до 1 (конец)

                // Если точка ветвления находится в запрещенной зоне, сдвигаем её в разрешенную зону
                if (branchingPoint < branchingPointThreshold)
                {
                    branchingPoint = branchingPointThreshold + (1 - branchingPointThreshold) * random.NextDouble();
                }

                double startX = x + (x2 - x) * branchingPoint;
                double startY = y + (y2 - y) * branchingPoint;

                // Угол и длина новой ветки
                double totalAngleSpread = baseAngle * 2; // Угол разброса
                double angleStep = totalAngleSpread / (currentBranchesPerNode - 1); // Шаг угла
                double newAngle = angle - baseAngle + i * angleStep;

                // Добавляем небольшое случайное отклонение
                newAngle += random.NextDouble() * 10 - 5;

                // Уменьшаем длину веток на каждом уровне
                double newLength = length * lengthReductionFactor;

                // Если это листья, рисуем их по-другому
                if (level >= 2)
                {
                    DrawPalmLeaf(startX, startY, newAngle, newLength);
                }
                else
                {
                    DrawPalmBranch(startX, startY, newAngle, newLength, level + 1, maxLevel);
                }
            }
        }

        private void DrawPalmLeaf(double x, double y, double angle, double length)
        {
            // Конечная точка листа
            double x2 = x + Math.Cos(angle * Math.PI / 180) * length;
            double y2 = y + Math.Sin(angle * Math.PI / 180) * length;

            // Рисуем лист пальмы
            Line leaf = new Line
            {
                X1 = x,
                Y1 = y,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };

            MainCanvas.Children.Add(leaf);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            DepthInput.Text = DepthInput.Text.Replace('.', ',');
            // Считываем параметры из текстовых полей
            if (int.TryParse(DepthInput.Text, out int depth))
                recursionDepth = depth;

            ProbabilityInput.Text = ProbabilityInput.Text.Replace('.', ',');
            if (double.TryParse(ProbabilityInput.Text, out double probability))
                branchProbability = Math.Clamp(probability, 0.0, 1.0); // Ограничиваем вероятность от 0 до 1

            AngleInput.Text = AngleInput.Text.Replace('.', ',');
            if (double.TryParse(AngleInput.Text, out double angle))
                baseAngle = angle;

            BranchesInput.Text = BranchesInput.Text.Replace('.', ',');
            if (int.TryParse(BranchesInput.Text, out int branches))
                branchesPerNode = Math.Max(2, branches); // Минимум 1 ветка

            TrunkLengthInput.Text = TrunkLengthInput.Text.Replace('.', ',');
            if (double.TryParse(TrunkLengthInput.Text, out double trunkLengthValue))
                trunkLength = trunkLengthValue;

            LengthReductionInput.Text = LengthReductionInput.Text.Replace('.', ',');
            if (double.TryParse(LengthReductionInput.Text, out double lengthReduction))
                lengthReductionFactor = Math.Clamp(lengthReduction, 0.1, 1.0); // Ограничиваем от 0.1 до 1.0

            BranchDensityInput.Text = BranchDensityInput.Text.Replace('.', ',');
            if (int.TryParse(BranchDensityInput.Text, out int branchDensity))
                branchDensityIncrease = Math.Max(1, branchDensity); // Минимум 1

            BranchingPointThresholdInput.Text = BranchingPointThresholdInput.Text.Replace('.', ',');
            if (double.TryParse(BranchingPointThresholdInput.Text, out double branchingPointThresholdValue))
                branchingPointThreshold = Math.Clamp(branchingPointThresholdValue, 0.1, 1.0); // Ограничиваем от 0.1 до 1.0

            DrawPalmTree();
        }
    }
}