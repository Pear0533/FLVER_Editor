using System.Numerics;
using SoulsFormats;

// ReSharper disable UnusedMember.Global

namespace FLVER_Editor;

public sealed class UVsPanel : Panel
{
    private readonly ComboBox _uvChannelSelector;
    private PointF offset = new(0, 0);

    public UVsPanel(ComboBox uvChannelSelector)
    {
        _uvChannelSelector = uvChannelSelector;
        _uvChannelSelector.SelectedIndexChanged += UvsChannelSelector_SelectedIndexChanged;
        DoubleBuffered = true;
    }

    public List<FLVER.Vertex> Vertices { get; set; } = new();
    public float Zoom { get; set; } = 1.0f;

    private void UvsChannelSelector_SelectedIndexChanged(object? sender, EventArgs e)
    {
        Invalidate();
    }

    public static List<Vector3> GetSelectedUVs(ComboBox uvChannelSelector, List<FLVER.Vertex> vertices)
    {
        int selectedIndex = uvChannelSelector.SelectedIndex;
        return vertices.Where(vertex => selectedIndex >= 0 && selectedIndex < vertex.UVs.Count)
            .Select(vertex => vertex.UVs[selectedIndex]).ToList();
    }

    public static Vector2 AdjustUV(Vector2 uv)
    {
        uv.X %= 1.0f;
        uv.Y %= 1.0f;
        if (uv.X < 0) uv.X += 1.0f;
        if (uv.Y < 0) uv.Y += 1.0f;
        return uv;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.ScaleTransform(Zoom, Zoom);
        List<Vector3> selectedUVs = GetSelectedUVs(_uvChannelSelector, Vertices);
        foreach (Vector3 uv in selectedUVs)
        {
            Vector2 adjustedUV = AdjustUV(new Vector2(uv.X, uv.Y));
            float x = adjustedUV.X * Width + offset.X;
            float y = adjustedUV.Y * Height + offset.Y;
            g.FillEllipse(Brushes.LawnGreen, x - 2, y - 2, 4, 4);
        }
    }

    public void SetZoom(float zoom)
    {
        Zoom = zoom;
        Invalidate();
    }

    public void Pan(PointF delta)
    {
        offset.X += delta.X;
        offset.Y += delta.Y;
        Invalidate();
    }
}