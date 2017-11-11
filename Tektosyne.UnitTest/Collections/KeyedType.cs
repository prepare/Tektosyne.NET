using System;
using System.Diagnostics;

using Tektosyne;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    public class KeyedType<TKey>: ICloneable, IMutableKeyedValue<TKey> {

        private readonly string _text;
        private TKey _key;

        public KeyedType(TKey key, string text) {
            _text = text;
            SetKey(key);
        }

        public TKey Key {
            [DebuggerStepThrough]
            get { return _key; }
        }

        public string Text {
            [DebuggerStepThrough]
            get { return _text; }
        }

        public object Clone() {
            return new KeyedType<TKey>(Key, Text);
        }

        public bool Equals(KeyedType<TKey> keyed) {
            if (keyed == null) return false;
            return (Key.Equals(keyed.Key) && Text == keyed.Text);
        }

        public override bool Equals(object obj) {
            return Equals(obj as KeyedType<TKey>);
        }

        public override int GetHashCode() {
            return (Text == null ? 0 : Text.GetHashCode());
        }

        public void SetKey(TKey key) {
            if (key == null)
                ThrowHelper.ThrowArgumentNullException("key");

            _key = key;
        }

        public override string ToString() {
            return StringUtility.Validate(Text);
        }
    }
}
