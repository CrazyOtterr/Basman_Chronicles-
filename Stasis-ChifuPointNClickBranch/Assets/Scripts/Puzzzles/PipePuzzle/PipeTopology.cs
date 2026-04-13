using UnityEngine;

/// <summary>
/// Маска открытых сторон трубы: N=1, E=2, S=4, W=8 (север = вверх по экрану).
/// Базовый спрайт при rotationQuarters = 0 (как в PipePuzzle):
/// Straight — лево/право (W|E); Corner — лево/низ (W|S); Tee — верх/лево/право (N|W|E), низ закрыт; Cross — все 4.
/// Клик крутит спрайт по часовой (−Z на RectTransform); маска крутится по часовой в том же смысле.
/// </summary>
public static class PipeTopology
{
    public const int N = 1;
    public const int E = 2;
    public const int S = 4;
    public const int W = 8;

    /// <summary>Базовая маска при rotationQuarters = 0 для каждого типа фигуры.</summary>
    public static int BaseMask(PipeShapeKind shape)
    {
        switch (shape)
        {
            case PipeShapeKind.Straight:
                return E | W; // горизонталь в исходном спрайте
            case PipeShapeKind.Corner:
                return S | W; // угол в исходном спрайте: юг + запад
            case PipeShapeKind.Tee:
                return N | E | W; // тройник, «ножка» вниз (S закрыт в базе)
            case PipeShapeKind.Cross:
                return N | E | S | W;
            case PipeShapeKind.Empty:
                return 0; // нет направлений — через клетку нельзя пройти
            default:
                return 0;
        }
    }

    /// <summary>Поворот по часовой стрелке: quarters от 0 до 3 (×90°).</summary>
    public static int MaskAfterRotation(PipeShapeKind shape, int rotationQuarters)
    {
        int m = BaseMask(shape);
        int q = ((rotationQuarters % 4) + 4) % 4;
        for (int i = 0; i < q; i++)
            m = RotateMaskClockwise(m);
        return m;
    }

    public static int RotateMaskClockwise(int mask)
    {
        bool n = (mask & N) != 0;
        bool e = (mask & E) != 0;
        bool s = (mask & S) != 0;
        bool w = (mask & W) != 0;
        // N←W, E←N, S←E, W←S
        int r = 0;
        if (w) r |= N;
        if (n) r |= E;
        if (e) r |= S;
        if (s) r |= W;
        return r;
    }

    public static bool HasOpenSide(int mask, int directionBit)
    {
        return (mask & directionBit) != 0;
    }

    /// <summary>Противоположная сторона соседа (если мы выходим на восток — у соседа должен быть запад).</summary>
    public static int Opposite(int directionBit)
    {
        if (directionBit == N) return S;
        if (directionBit == E) return W;
        if (directionBit == S) return N;
        if (directionBit == W) return E;
        return 0;
    }
}

public enum PipeShapeKind
{
    Straight,
    Corner,
    Tee,
    Cross,
    /// <summary>Пустая клетка: ни одного открытого направления, путь не проходит.</summary>
    Empty
}
