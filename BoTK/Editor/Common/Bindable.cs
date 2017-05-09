namespace BoTK.Editor.Common {
  public interface Bindable {
    void Bind(DrawCall call);
    void Unbind(DrawCall call);
  }
}