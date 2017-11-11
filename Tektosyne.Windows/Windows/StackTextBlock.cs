using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides a <see cref="TextBlock"/> with a message stack.</summary>
    /// <remarks>
    /// <b>StackTextBlock</b> enhances <see cref="TextBlock"/> with a <see cref="Stack{String}"/> of
    /// recently displayed messages. This simplifies managing status messages in a WPF application,
    /// especially when multiple updates by nested method calls are involved.</remarks>

    public class StackTextBlock: TextBlock {
        #region StackTextBlock()

        /// <overloads>
        /// Initializes a new instance of the <see cref="StackTextBlock"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="StackTextBlock"/> class with default
        /// properties.</summary>

        public StackTextBlock() { }

        #endregion
        #region StackTextBlock(String)

        /// <summary>
        /// Initializes a new instance of the <see cref="StackTextBlock"/> class with the specified
        /// initial and default <see cref="TextBlock.Text"/>.</summary>
        /// <param name="text">
        /// The initial and default <see cref="TextBlock.Text"/> for the <see
        /// cref="StackTextBlock"/>.</param>

        public StackTextBlock(string text): base() {
            _defaultText = text;
            Text = text;
        }

        #endregion
        #region Private Fields

        // default Text when stack is empty
        private string _defaultText;

        // stack with most recent Text at the top
        private readonly Stack<String> _textStack = new Stack<String>();

        #endregion
        #region DefaultText

        /// <summary>
        /// Gets or sets the default <see cref="TextBlock.Text"/> for the <see
        /// cref="StackTextBlock"/>.</summary>
        /// <value>
        /// The default <see cref="TextBlock.Text"/> that the <see cref="StackTextBlock"/> shows
        /// when the message stack is empty.</value>
        /// <remarks><para>
        /// <b>DefaultText</b> returns an empty string when set to a null reference.
        /// </para><para>
        /// Setting <b>DefaultText</b> also updates the current <see cref="TextBlock.Text"/> if the
        /// message stack is empty.</para></remarks>

        public string DefaultText {
            [DebuggerStepThrough]
            get { return _defaultText ?? ""; }
            set {
                _defaultText = value;
                if (_textStack.Count == 0)
                    Text = value;
            }
        }

        #endregion
        #region Clear

        /// <summary>
        /// Removes all entries from the message stack and shows the <see cref="DefaultText"/>.
        /// </summary>
        /// <remarks>
        /// <b>Clear</b> removes all entries from the message stack and sets the <see
        /// cref="TextBlock.Text"/> property to <see cref="DefaultText"/>.</remarks>

        public void Clear() {
            _textStack.Clear();
            Text = DefaultText;
        }

        #endregion
        #region Peek

        /// <summary>
        /// Returns the entry at the top of the message stack without removing it.</summary>
        /// <returns>
        /// The entry at the top of the message stack.</returns>
        /// <remarks>
        /// <b>Peek</b> returns the <see cref="TextBlock.Text"/> that would be shown if <see
        /// cref="Pop"/> was invoked on the <see cref="StackTextBlock"/>. That is either the entry
        /// at the top of the message stack, or <see cref="DefaultText"/> if the stack is empty.
        /// </remarks>

        public string Peek() {
            if (_textStack.Count == 0)
                return DefaultText;
            else
                return _textStack.Peek();
        }

        #endregion
        #region Pop

        /// <summary>
        /// Removes and shows the entry at the top of the message stack.</summary>
        /// <returns>
        /// The current <see cref="TextBlock.Text"/> before <b>Pop</b> was invoked.</returns>
        /// <remarks>
        /// <b>Pop</b> removes the entry at the top of the message stack, assigns it to the <see
        /// cref="TextBlock.Text"/> property, and returns the previous value of that property. If
        /// the message stack is empty, <b>Text</b> is set to <see cref="DefaultText"/>.</remarks>

        public string Pop() {
            string oldText = Text;

            if (_textStack.Count == 0)
                Text = DefaultText;
            else
                Text = _textStack.Pop();

            return oldText;
        }

        #endregion
        #region Push()

        /// <overloads>
        /// Inserts the current <see cref="TextBlock.Text"/> at the top of the message stack.
        /// </overloads>
        /// <summary>
        /// Inserts the current <see cref="TextBlock.Text"/> at the top of the message stack.
        /// </summary>
        /// <remarks>
        /// <b>Push</b> inserts the current value of the <see cref="TextBlock.Text"/> property at
        /// the top of the message stack, without changing the <b>Text</b> property itself. Call
        /// this overload if you wish to maintain the current <see cref="TextBlock.Text"/> beyond a
        /// subsequent call to <see cref="Pop"/>.</remarks>

        public void Push() {
            _textStack.Push(Text);
        }

        #endregion
        #region Push(String)

        /// <summary>
        /// Inserts the current <see cref="TextBlock.Text"/> at the top of the message stack and
        /// shows the specified <b>Text</b>.</summary>
        /// <param name="text">
        /// The new value for the <see cref="TextBlock.Text"/> property.</param>
        /// <remarks>
        /// <b>Push</b> inserts the current value of the <see cref="TextBlock.Text"/> property at
        /// the top of the message stack, and then sets the <b>Text</b> property to the specified
        /// <paramref name="text"/>. Call this overload whenever you need to show a new <see
        /// cref="TextBlock.Text"/>, and then call <see cref="Pop"/> at a later time to restore the
        /// previous <b>Text</b>.</remarks>

        public void Push(string text) {
            Push();
            Text = text ?? "";
        }

        #endregion
    }
}
