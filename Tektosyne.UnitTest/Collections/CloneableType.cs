using System;
using System.Diagnostics;

namespace Tektosyne.UnitTest.Collections {

    public class CloneableType: ICloneable {

        private readonly string _text;

        public CloneableType(string text) {
            _text = text;
        }

        public string Text {
            [DebuggerStepThrough]
            get { return _text; }
        }

        public object Clone() {
            return new CloneableType(Text);
        }

        public bool Equals(CloneableType cloneable) {
            if (cloneable == null) return false;
            return (Text == cloneable.Text);
        }

        public override bool Equals(object obj) {
            return Equals(obj as CloneableType);
        }

        public override int GetHashCode() {
            return (Text == null ? 0 : Text.GetHashCode());
        }

        public override string ToString() {
            return StringUtility.Validate(Text);
        }

        public static implicit operator CloneableType(String text) {
            return new CloneableType(text);
        }

        public static explicit operator String(CloneableType cloneable) {
            return cloneable.Text;
        }
    }
}
