using GraphEditor.Models;

namespace GraphEditor.Algorithms;

public static class ExtensionMethods
{
    public static (int, int) Swap(this (int, int) value) => (value.Item2, value.Item1);
}