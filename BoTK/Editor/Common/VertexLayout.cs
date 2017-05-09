using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Vector3 = BotWLib.Types.Vector3;

namespace BoTK.Editor.Common {
  public class AttributeSlotMeta : Attribute {
    public readonly int Index;

    public readonly string Name;
    public readonly VertexAttribPointerType PointerType;

    public readonly int Count;

    public AttributeSlotMeta(int index, string name, VertexAttribPointerType pointerType, int count) {
      Index = index;

      Name = name;
      PointerType = pointerType;

      Count = count;
    }
  }

  public struct AttributeSlot {
    public string Name;

    public int Size;
    public int Offset;
    public int Stride;

    public VertexAttribPointerType PointerType;
  }

  public class VertexLayout : Bindable {
    public int Size = 0;
    public List<AttributeSlot> attributeSlots = new List<AttributeSlot>();

    public VertexLayout(Type type) {
      var slotMetadata = type
        .GetMembers()
        .Where(m => m.GetCustomAttributes<AttributeSlotMeta>().Any())
        .Select(m => m.GetCustomAttribute(typeof(AttributeSlotMeta)) as AttributeSlotMeta)
        .Distinct();

      int runningSize = 0;
      foreach (var slotMeta in slotMetadata) {
        var elementSize = GetAttribPointerSize(slotMeta.PointerType) * slotMeta.Count;

        Size += elementSize;
      }

      foreach (var slotMeta in slotMetadata) {
        var elementSize = GetAttribPointerSize(slotMeta.PointerType) * slotMeta.Count;

        var slot = new AttributeSlot {
          Name = slotMeta.Name,
          Size = slotMeta.Count,
          Offset = runningSize,
          PointerType = slotMeta.PointerType,

          Stride = Size
        };

        attributeSlots.Add(slot);

        runningSize += elementSize;
      }
    }

    public void Apply(Shader shader) {

    }

    private static int GetAttribPointerSize(VertexAttribPointerType pointerType) {
      if (pointerType == VertexAttribPointerType.Byte ||
          pointerType == VertexAttribPointerType.UnsignedByte)
        return 1;

      if (pointerType == VertexAttribPointerType.Short ||
          pointerType == VertexAttribPointerType.UnsignedShort)
        return 2;

      if (pointerType == VertexAttribPointerType.Int ||
          pointerType == VertexAttribPointerType.UnsignedInt ||
          pointerType == VertexAttribPointerType.Float)
        return 4;

      if (pointerType == VertexAttribPointerType.Double)
        return 8;

      return 0;
    }

    public void Bind(DrawCall call) {
      foreach (var slot in attributeSlots) {
        var location = call.shader.GetAttributeLocation(slot.Name);

        if (location != -1) {
          GL.VertexAttribPointer(
            location, slot.Size, slot.PointerType, false,
            slot.Stride, slot.Offset
          );

          GL.EnableVertexAttribArray(location);
        }
      }
    }

    public void Unbind(DrawCall call) {
      foreach (var slot in attributeSlots) {
        var location = call.shader.GetAttributeLocation(slot.Name);

        if (location != -1)
          GL.DisableVertexAttribArray(location);
      }
    }
  }
}