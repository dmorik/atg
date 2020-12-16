using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGT
{
    public class Graph
    {
        private bool[,] _values;
        private int _dimensions;

        public Graph()
        {
            Clear();
        }

        public void Clear()
        {
            _dimensions = 0;
            _values = new bool[0, 0];
        }

        public void AddVertex(int a)
        {
            if (a <= _dimensions)
            {
                return;
            }

            var newValues = new bool[a, a];

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    newValues[i, j] = _values[i, j];
                }
            }

            _dimensions = a;
            _values = newValues;
        }

        public void DeleteVertex(int a)
        {
            if (a > _dimensions)
            {
                return;
            }

            var newDimensions = _dimensions - 1;
            var newValues = new bool[newDimensions, newDimensions];

            var y = -1;
            for (int i = 0; i < _dimensions; i++)
            {
                if (i == a - 1)
                {
                    continue;
                }
                y++;

                var x = -1;
                for (int j = 0; j < _dimensions; j++)
                {
                    if (j == a - 1)
                    {
                        continue;
                    }
                    x++;

                    newValues[y, x] = _values[i, j];
                }
            }

            _dimensions = newDimensions;
            _values = newValues;
        }

        public Graph DecartMul(Graph otherMatrix)
        {
            var result = new Graph();

            var vertices = new List<Tuple<int, int>>();

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < otherMatrix._dimensions; j++)
                {
                    vertices.Add(Tuple.Create(i, j));
                }
            }

            result._dimensions = vertices.Count;
            result._values = new bool[vertices.Count, vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (vertices[i].Item1 == vertices[j].Item1
                        && otherMatrix._values[vertices[i].Item2, vertices[j].Item2]
                        || vertices[i].Item2 == vertices[j].Item2
                        && _values[vertices[i].Item1, vertices[j].Item1])
                    {
                        result._values[i, j] = true;
                        result._values[j, i] = true;
                    }
                }
            }

            return result;
        }

        public Graph TensorMul(Graph otherMatrix)
        {
            var result = new Graph();

            var vertices = new List<Tuple<int, int>>();

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < otherMatrix._dimensions; j++)
                {
                    vertices.Add(Tuple.Create(i, j));
                }
            }

            result._dimensions = vertices.Count;
            result._values = new bool[vertices.Count, vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (_values[vertices[i].Item1, vertices[j].Item1]
                        && otherMatrix._values[vertices[i].Item2, vertices[j].Item2])
                    {
                        result._values[i, j] = true;
                        result._values[j, i] = true;
                    }
                }
            }

            return result;
        }

        public void Print(string filePath)
        {
            if (!filePath.EndsWith(".png"))
            {
                throw new Exception($"Файл должен иметь расширение .png ({filePath})");
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var plotter = new ScottPlot.Plot(1080, 1080);

            var xPoints = new List<double>();
            var yPoints = new List<double>();

            plotter.Axis(-1.25, 1.25, -1.25, 1.25);
            plotter.XTicks(new double[0], new string[0]);
            plotter.YTicks(new double[0], new string[0]);
            plotter.Frame(left: false, right: false, top: false);
            plotter.TightenLayout(padding: 0, render: true);

            if (_dimensions != 0)
            {
                if (_dimensions == 1)
                {
                    xPoints.Add(0.0);
                    yPoints.Add(0.0);
                }
                else
                {
                    for (int i = 0; i < _dimensions; i++)
                    {
                        xPoints.Add(Math.Cos(2.0 * Math.PI / _dimensions * i));
                        yPoints.Add(Math.Sin(2.0 * Math.PI / _dimensions * i));
                    }
                }

                var k = -12.5 / 18.0;
                var b = 15.0 - 2.0 * k;
                var lineWidth = k * _dimensions + b;

                for (int i = 0; i < _dimensions; i++)
                {
                    for (int j = 0; j < _dimensions; j++)
                    {
                        if (_values[i, j] == true)
                        {
                            plotter.PlotLine(xPoints[i], yPoints[i], xPoints[j], yPoints[j], color: System.Drawing.Color.Blue, lineWidth: lineWidth);
                        }
                    }
                }

                plotter.PlotScatter(xPoints.ToArray(), yPoints.ToArray(), color: System.Drawing.Color.Blue, markerSize: 50.0, lineStyle: ScottPlot.LineStyle.None);
                plotter.PlotScatter(xPoints.ToArray(), yPoints.ToArray(), color: System.Drawing.Color.Orange, markerSize: 40.0, lineStyle: ScottPlot.LineStyle.None);
            }

            plotter.SaveFig(filePath);
        }

        public string PrintMatrix()
        {
            var resultString = "";

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    resultString += _values[i, j] ? "1" : "0";
                }

                resultString += $"{Environment.NewLine}";
            }

            if (resultString == "")
            {
                return "{ пустая }";
            }

            return resultString;
        }

        public Graph EdgesGraph()
        {
            var result = new Graph();

            var edges = new List<Tuple<int, int>>();

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = i + 1; j < _dimensions; j++)
                {
                    if (_values[i, j] == true)
                    {
                        edges.Add(Tuple.Create(i, j));
                    }
                }
            }

            result._dimensions = edges.Count;
            result._values = new bool[edges.Count, edges.Count];

            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = 0; j < edges.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (edges[i].Item1 == edges[j].Item1
                        || edges[i].Item1 == edges[j].Item2
                        || edges[i].Item2 == edges[j].Item1
                        || edges[i].Item2 == edges[j].Item2)
                    {
                        result._values[i, j] = true;
                        result._values[j, i] = true;
                    }
                }
            }

            return result;
        }

        public void AddEdge(int a, int b)
        {
            AddVertex(Math.Max(a, b));

            if (a == b)
            {
                return;
            }

            _values[a - 1, b - 1] = true;
            _values[b - 1, a - 1] = true;
        }

        public Graph Sum(Graph otherMatrix)
        {
            var result = new Graph();

            var newDimensions = _dimensions + otherMatrix._dimensions;
            var newValues = new bool[newDimensions, newDimensions];

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    newValues[i, j] = _values[i, j];
                }
            }

            for (int i = _dimensions; i < newDimensions; i++)
            {
                for (int j = _dimensions; j < newDimensions; j++)
                {
                    newValues[i, j] = otherMatrix._values[i - _dimensions, j - _dimensions];
                }
            }

            result._dimensions = newDimensions;
            result._values = newValues;

            return result;
        }

        public static Graph LoadFromString(string matrix)
        {
            var result = new Graph();

            if (matrix == "{ пустая }")
            {
                return result;
            }

            var matrixLength = matrix.Length;
            var matrixSide = 0;

            for (int i = 1; i < (int)(Math.Sqrt(matrixLength) + 1.0); i++)
            {
                if (i * i == matrixLength)
                {
                    matrixSide = i;

                    break;
                }
            }

            if (matrixSide == 0)
            {
                throw new Exception($"Неверная длина входа");
            }

            result._dimensions = matrixSide;
            result._values = new bool[matrixSide, matrixSide];

            for (int i = 0; i < matrixSide; i++)
            {
                for (int j = 0; j < matrixSide; j++)
                {
                    if (matrix[i * matrixSide + j] == '0')
                    {
                        result._values[i, j] = false;
                    }
                    else if (matrix[i * matrixSide + j] == '1')
                    {
                        result._values[i, j] = true;
                    }
                    else
                    {
                        throw new Exception($"Неизестный символ ({matrix[i * matrixSide + j]})");
                    }
                }
            }

            for (int i = 0; i < matrixSide; i++)
            {
                for (int j = 0; j < matrixSide; j++)
                {
                    if (i == j
                        && result._values[i, j] == true)
                    {
                        throw new Exception($"В загружаемом графе не должно быть петель");
                    }

                    if (result._values[i, j] != result._values[j, i])
                    {
                        throw new Exception($"Матрица смежности загружаемого графа должна быть симметричной");
                    }
                }
            }

            return result;
        }

        public Graph Addition()
        {
            var result = new Graph();

            result._values = new bool[_dimensions, _dimensions];

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    result._values[i, j] = !_values[i, j];
                }
            }

            result._dimensions = _dimensions;

            return result;
        }

        public void DeleteEdge(int a, int b)
        {
            if (a > _dimensions || b > _dimensions)
            {
                return;
            }

            _values[a - 1, b - 1] = false;
            _values[b - 1, a - 1] = false;
        }

        public int EdgeCount()
        {
            var result = 0;

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = i + 1; j < _dimensions; j++)
                {
                    if (_values[i, j])
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        public string EdgeList()
        {
            var resultString = "{";

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = i + 1; j < _dimensions; j++)
                {
                    if (_values[i, j])
                    {
                        resultString += $" [{i + 1}, {j + 1}]";
                    }
                }
            }

            return resultString + " }";
        }

        public override string ToString()
        {
            if (_dimensions == 0)
            {
                return $"{{ пустой }}{Environment.NewLine}";
            }

            var resultString = "";

            for (int i = 0; i < _dimensions; i++)
            {
                resultString += $"{i + 1}:";

                for (int j = 0; j < _dimensions; j++)
                {
                    if (_values[i, j] == true)
                    {
                        resultString += $" {j + 1},";
                    }
                }

                if (resultString.EndsWith(","))
                {
                    resultString = resultString.Substring(0, resultString.Length - 1);
                }
                else
                {
                    resultString += " -";
                }

                resultString += $"{Environment.NewLine}";
            }

            return resultString;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var A = new Graph();
            var B = new Graph();

            while (true)
            {
                try
                {
                    Console.Clear();

                    Console.WriteLine("Граф A:");
                    Console.WriteLine(A);

                    Console.WriteLine("Граф B");
                    Console.WriteLine(B);

                    Console.WriteLine($"Для вызова подсказки введите команду \"help\"");
                    Console.WriteLine($"Для выхода из программы  введите команду \"exit\"");
                    Console.WriteLine();

                    Console.WriteLine("Введите команду:");
                    Console.Write(">> ");

                    var inputString = Console.ReadLine().Trim();

                    if (inputString == "")
                    {
                        continue;
                    }

                    var commandParts = inputString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    var command = commandParts.First();

                    if (command == "exit")
                    {
                        break;
                    }

                    switch (command)
                    {
                        case "help":
                            {
                                Console.WriteLine("Список доступных команд:");
                                Console.WriteLine();

                                Console.WriteLine($"\"clear\" - очистить граф (clear A)");
                                Console.WriteLine($"\"add_edge\" - добавить ребро в граф (add_edge A 1 2)");
                                Console.WriteLine($"\"delete_edge\" - удалить ребро из графа (delete_edge A 1 2)");
                                Console.WriteLine($"\"add_vertex\" - добавить вершину в граф (add_vertex A 2)");
                                Console.WriteLine($"\"delete_vertex\" - удалить вершину из графа (delete_vertex A 2)");
                                Console.WriteLine($"\"edge_count\" - вывести число ребер графа (edge_count A)");
                                Console.WriteLine($"\"edge_list\" - вывести список всех ребер графа (edge_list A)");
                                Console.WriteLine($"\"addition\" - найти дополнение графа (addition A A)");
                                Console.WriteLine($"\"matrix\" - вывести матрицу смежности графа (matrix A)");
                                Console.WriteLine($"\"sum\" - найти прямую сумму графов (sum A B A)");
                                Console.WriteLine($"\"save\" - сохранить матрицу смежности в файл (save A A.txt)");
                                Console.WriteLine($"\"load\" - загрузить граф по матрице смежности из файла (load A A.txt)");
                                Console.WriteLine($"\"edges_graph\" - найти реберный граф по графу (edges_graph A A)");
                                Console.WriteLine($"\"print\" - визуализировать граф (print A A.png)");
                                Console.WriteLine($"\"decart_mul\" - найти декартово произведение графов (decart_mul A B A)");
                                Console.WriteLine($"\"tensor_mul\" - найти тезнорное произведение графов (tensor_mul A B A)");
                                

                                Console.WriteLine();
                                Console.Write($"Нажмите любую кнопку для продолжения работы");

                                Console.ReadKey();

                                break;
                            }
                        case "clear":
                            {
                                if (commandParts.Length != 2)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры clear, пример вызова команды: (clear A)");
                                }

                                var commandArg = commandParts[1];

                                if (commandArg == "A")
                                {
                                    A.Clear();
                                }
                                else if (commandArg == "B")
                                {
                                    B.Clear();
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент процедуры clear ({commandArg}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "add_edge":
                            {
                                if (commandParts.Length != 4)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры add_edge, пример команды (add_edge A 1 2)");
                                }

                                var firstArg = commandParts[1];

                                if (!int.TryParse(commandParts[2], out var sourceVertex))
                                {
                                    throw new Exception($"Ошибка считывания числа из ({commandParts[2]})");
                                }

                                if (!int.TryParse(commandParts[3], out var targetVertex))
                                {
                                    throw new Exception($"Ошибка считывания числа из ({commandParts[3]})");
                                }

                                if (firstArg == "A")
                                {
                                    A.AddEdge(sourceVertex, targetVertex);
                                }
                                else if (firstArg == "B")
                                {
                                    B.AddEdge(sourceVertex, targetVertex);
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент процедуры add_edge ({firstArg}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "add_vertex":
                            {
                                if (commandParts.Length != 3)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры add_vertex, пример команды (add_vertex A 2)");
                                }

                                var graphName = commandParts[1];

                                if (!int.TryParse(commandParts[2], out var vertex))
                                {
                                    throw new Exception($"Ошибка считывания числа из ({commandParts[2]})");
                                }

                                if (graphName == "A")
                                {
                                    A.AddVertex(vertex);
                                }
                                else if (graphName == "B")
                                {
                                    B.AddVertex(vertex);
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры add_vertex ({graphName}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "delete_vertex":
                            {
                                if (commandParts.Length != 3)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры delete_vertex, пример вызова команды (delete_vertex A 2 3)");
                                }

                                var graphName = commandParts[1];

                                if (!int.TryParse(commandParts[2], out var sourceVertex))
                                {
                                    throw new Exception($"Ошибка считывания числа из ({commandParts[2]})");
                                }

                                if (graphName == "A")
                                {
                                    A.DeleteVertex(sourceVertex);
                                }
                                else if (graphName == "B")
                                {
                                    B.DeleteVertex(sourceVertex);
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры delete_vertex ({graphName}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "delete_edge":
                            {
                                if (commandParts.Length != 4)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры delete_edge, пример вызова команды (delete_edge A 2 3)");
                                }

                                var graphName = commandParts[1];

                                if (!int.TryParse(commandParts[2], out var sourceVertex))
                                {
                                    throw new Exception($"Ошибка считывания числа из ({commandParts[2]})");
                                }

                                if (!int.TryParse(commandParts[3], out var targetVertex))
                                {
                                    throw new Exception($"Ошибка считывания числа из ({commandParts[3]})");
                                }

                                if (graphName == "A")
                                {
                                    A.DeleteEdge(sourceVertex, targetVertex);
                                }
                                else if (graphName == "B")
                                {
                                    B.DeleteEdge(sourceVertex, targetVertex);
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры graph_name ({graphName}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "edge_count":
                            {
                                if (commandParts.Length != 2)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры edge_count, пример вызова команды (edge_count A)");
                                }

                                var graphName = commandParts[1];

                                if (graphName == "A")
                                {
                                    Console.WriteLine($"Количество ребер графа A: {A.EdgeCount()}");
                                    Console.WriteLine();
                                    Console.Write($"Нажмите любую кнопку для продолжения работы");
                                    Console.ReadKey();
                                }
                                else if (graphName == "B")
                                {
                                    Console.WriteLine($"Количество ребер графа B: {B.EdgeCount()}");
                                    Console.WriteLine();
                                    Console.Write($"Нажмите любую кнопку для продолжения работы");
                                    Console.ReadKey();
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры edge_count, доступные аргументы: A, B");
                                }

                                break;
                            }
                        case "edge_list":
                            {
                                if (commandParts.Length != 2)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры edge_list, пример команды (edge_list A)");
                                }

                                var graphName = commandParts[1];

                                if (graphName == "A")
                                {
                                    Console.WriteLine($"Ребра графа A: {A.EdgeList()}");
                                    Console.WriteLine();
                                    Console.Write($"Нажмите любую кнопку для продолжения работы");
                                    Console.ReadKey();
                                }
                                else if (graphName == "B")
                                {
                                    Console.WriteLine($"Ребра графа B: {B.EdgeList()}");
                                    Console.WriteLine();
                                    Console.Write($"Нажмите любую кнопку для продолжения работы");
                                    Console.ReadKey();
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры edge_list, доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "addition":
                            {
                                if (commandParts.Length != 3)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры addition, пример вызова команды (addition A)");
                                }

                                var sourceGraphName = commandParts[1];
                                var targetGraphName = commandParts[2];

                                var addition = null as Graph;
                                if (sourceGraphName == "A")
                                {
                                    addition = A.Addition();
                                }
                                else if (sourceGraphName == "B")
                                {
                                    addition = B.Addition();
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры addition ({sourceGraphName}), доступны аргументы: A, B");
                                }

                                if (targetGraphName == "A")
                                {
                                    A = addition;
                                }
                                else if (targetGraphName == "B")
                                {
                                    B = addition;

                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры addition ({targetGraphName}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "matrix":
                            {
                                if (commandParts.Length != 2)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры matrux, пример вызова команды (matrix A)");
                                }

                                var graphName = commandParts[1];

                                if (graphName == "A")
                                {
                                    Console.WriteLine($"Матрица смежности графа A:");
                                    Console.WriteLine();
                                    Console.Write(A.PrintMatrix());
                                    Console.WriteLine();
                                    Console.WriteLine($"Для продолжения нажмите любую кнопку");
                                    Console.ReadKey();
                                }
                                else if (graphName == "B")
                                {
                                    Console.WriteLine($"Матрица смежности графа B:");
                                    Console.WriteLine();
                                    Console.WriteLine(B.PrintMatrix());
                                    Console.WriteLine();
                                    Console.Write($"Для продолжения нажмите любую кнопку");
                                    Console.ReadKey();
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры matrix ({graphName}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "sum":
                            {
                                if (commandParts.Length != 4)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры sum, пример вызова команды (sum A B A)");
                                }

                                var firstGraphName = commandParts[1];
                                var secondGraphName = commandParts[2];
                                var targetGraphName = commandParts[3];

                                var firstArg = null as Graph;
                                if (firstGraphName == "A")
                                {
                                    firstArg = A;
                                }
                                else if (secondGraphName == "B")
                                {
                                    firstArg = B;
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры sum ({firstGraphName}), доступны аргуметы: A, B");
                                }

                                var secondArg = null as Graph;
                                if (secondGraphName == "A")
                                {
                                    secondArg = A;
                                }
                                else if (secondGraphName == "B")
                                {
                                    secondArg = B;
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры sum ({secondGraphName}), доступны аргументы: A, B");
                                }

                                if (targetGraphName == "A")
                                {
                                    A = firstArg.Sum(secondArg);
                                }
                                else if (targetGraphName == "B")
                                {
                                    B = firstArg.Sum(secondArg);
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры sum ({targetGraphName}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "save":
                            {
                                if (commandParts.Length != 3)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры save, пример вызова команды (save A A_matrix.txt)");
                                }

                                var graphName = commandParts[1];
                                var filePath = commandParts[2];

                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }

                                if (graphName == "A")
                                {
                                    try
                                    {
                                        File.AppendAllText(filePath, A.PrintMatrix());
                                    }
                                    catch
                                    {
                                        throw new Exception($"Ошибка при сохранении матрицы смежности в файл ({filePath})");
                                    }

                                    Console.WriteLine($"Мартрица смежности графа A успешно сохранена в ({filePath})");
                                    Console.WriteLine();
                                    Console.Write($"Для продолжения работы нажмите любую кнопку");
                                    Console.ReadKey();
                                }
                                else if (graphName == "B")
                                {
                                    try
                                    {
                                        File.AppendAllText(filePath, B.PrintMatrix());
                                    }
                                    catch
                                    {
                                        throw new Exception($"Ошибка при сохранении матрицы смежности в файл ({filePath})");
                                    }

                                    Console.WriteLine($"Мартрица смежности графа B успешно сохранена в ({filePath})");
                                    Console.WriteLine();
                                    Console.Write($"Для продолжения работы нажмите любую кнопку");
                                    Console.ReadKey();
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры save ({graphName}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "load":
                            {
                                if (commandParts.Length != 3)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры load, пример вызова команды (load A A_matrix.txt)");
                                }

                                var graphName = commandParts[1];
                                var filePath = commandParts[2];

                                if (!File.Exists(filePath))
                                {
                                    throw new Exception($"Файл не найден ({filePath})");
                                }

                                var fileData = "";
                                try
                                {
                                    fileData = string.Join("", File.ReadAllLines(filePath));
                                }
                                catch
                                {
                                    throw new Exception($"Ошибка считывания файла ({filePath})");
                                }

                                if (graphName == "A")
                                {
                                    A = Graph.LoadFromString(fileData);
                                }
                                else if (graphName == "B")
                                {
                                    B = Graph.LoadFromString(fileData);
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры load ({graphName}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "edges_graph":
                            {
                                if (commandParts.Length != 3)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры edges_graph, пример вызова команды (edges_graph A A)");
                                }

                                var sourceGraphName = commandParts[1];
                                var targetGraphName = commandParts[2];

                                var sourceGraph = null as Graph;
                                if (sourceGraphName == "A")
                                {
                                    sourceGraph = A;
                                }
                                else if (sourceGraphName == "B")
                                {
                                    sourceGraph = B;
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедур edges_graph ({sourceGraphName}), доступны аргументы: A, B");
                                }

                                if (targetGraphName == "A")
                                {
                                    A = sourceGraph.EdgesGraph();
                                }
                                else if (targetGraphName == "B")
                                {
                                    B = sourceGraph.EdgesGraph();
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедур edges_graph ({targetGraphName}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "print":
                            {
                                if (commandParts.Length != 3)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры print, пример вызова команды (print A A_matrix.png)");
                                }

                                var graphName = commandParts[1];
                                var filePath = commandParts[2];

                                var printGraph = null as Graph;
                                if (graphName == "A")
                                {
                                    A.Print(filePath);
                                }
                                else if (graphName == "B")
                                {
                                    B.Print(filePath);
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры print, доступны аргументы: A, B");
                                }

                                Console.WriteLine($"Иллюстрация графа была успешна сохранена в \"{filePath}\"");
                                Console.WriteLine();
                                Console.Write($"Нажмите любую кнопку для продолжения работы");
                                Console.ReadKey();

                                break;
                            }
                        case "decart_mul":
                            {
                                if (commandParts.Length != 4)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры decart_mul, пример вызова команды (decart_mul A B A)");
                                }

                                var firstGraphName = commandParts[1];
                                var secondGraphName = commandParts[2];

                                var firstGraph = null as Graph;
                                if (firstGraphName == "A")
                                {
                                    firstGraph = A;
                                }
                                else if (firstGraphName == "B")
                                {
                                    firstGraph = B;
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры decart_mul ({firstGraphName}), доступны аргументы: A, B");
                                }

                                var secondGraph = null as Graph;
                                if (secondGraphName == "A")
                                {
                                    secondGraph = A;
                                }
                                else if (secondGraphName == "B")
                                {
                                    secondGraph = B;
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры decart_mul ({firstGraphName}), доступны аргументы: A, B");
                                }

                                var targetGraphName = commandParts[3];

                                if (targetGraphName == "A")
                                {
                                    A = firstGraph.DecartMul(secondGraph);
                                }
                                else if (targetGraphName == "B")
                                {
                                    B = firstGraph.DecartMul(secondGraph);
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры decart_mul ({firstGraphName}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        case "tensor_mul":
                            {
                                if (commandParts.Length != 4)
                                {
                                    throw new Exception($"Неверное число аргументов для вызова процедуры tensor_mul, пример вызова команды (tensor_mul A B A)");
                                }

                                var firstGraphName = commandParts[1];
                                var secondGraphName = commandParts[2];

                                var firstGraph = null as Graph;
                                if (firstGraphName == "A")
                                {
                                    firstGraph = A;
                                }
                                else if (firstGraphName == "B")
                                {
                                    firstGraph = B;
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры tensor_mul ({firstGraphName}), доступны аргументы: A, B");
                                }

                                var secondGraph = null as Graph;
                                if (secondGraphName == "A")
                                {
                                    secondGraph = A;
                                }
                                else if (secondGraphName == "B")
                                {
                                    secondGraph = B;
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры tensor_mul ({firstGraphName}), доступны аргументы: A, B");
                                }

                                var targetGraphName = commandParts[3];

                                if (targetGraphName == "A")
                                {
                                    A = firstGraph.TensorMul(secondGraph);
                                }
                                else if (targetGraphName == "B")
                                {
                                    B = firstGraph.TensorMul(secondGraph);
                                }
                                else
                                {
                                    throw new Exception($"Неизвестный аргумент для вызова процедуры tensor_mul ({firstGraphName}), доступны аргументы: A, B");
                                }

                                break;
                            }
                        default:
                            {
                                throw new Exception($"Неизвестная команда ({command})");
                            }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{Environment.NewLine}Ошибка: {ex.Message}");
                    Console.WriteLine();
                    Console.Write($"Нажмите любую клавишу для продолжения работы");
                    Console.ReadKey();
                }
            }
        }
    }
}
