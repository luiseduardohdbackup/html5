using System;
using System.Text;
using System.Collections.Generic;

namespace Html5.Tokenizer
{
	public enum TokenType
	{
		Doctype,
		Comment,
		Character,
		StartTag,
		EndTag,
		EndOfFile,
	}

	public enum TokenFlags
	{
		None = 0,
		ForceQuirks = 1
	}

	public class DoctypeToken : Token {
		public DoctypeToken (bool forceQuirks) : base (TokenType.Doctype)
		{
			SetFlag (TokenFlags.ForceQuirks, forceQuirks);
			_name = new StringBuilder ();
		}

		StringBuilder _name;

		public void AppendName (int ch)
		{
			if ('A' <= ch && ch <= 'Z') {
				ch -= 0x20;
			}
			_name.Append ((char)ch);
		}

		StringBuilder _publicIdentifier;

		public void SetEmptyPublicIdentifier ()
		{
			_publicIdentifier = new StringBuilder ();
		}

		public void AppendPublicIdentifier (int ch)
		{
			if (_publicIdentifier == null) {
				_publicIdentifier = new StringBuilder();
			}
			_publicIdentifier.Append ((char)ch);
		}
	}

	public abstract class TagToken : Token {
		public TagToken (TokenType type, int ch) : base (type)
		{
			_name = new StringBuilder ();
			AppendName (ch);
		}

		public string Name { get { return _name.ToString(); } }

		StringBuilder _name;

		public void AppendName (int ch)
		{
			if ('A' <= ch && ch <= 'Z') {
				ch -= 0x20;
			}
			_name.Append ((char)ch);
		}

		List<Attribute> _attributes;

		public void AddAttribute (Attribute attr)
		{
			if (_attributes == null) {
				_attributes = new List<Attribute>();
			}
			_attributes.Add (attr);
		}

		public Attribute CurrentAttribute {
			get { return (_attributes == null) ? null :
					_attributes[_attributes.Count - 1]; }
		}
	}

	public class StartTagToken : TagToken {
		public StartTagToken (int ch) : base (TokenType.EndTag, ch)
		{
		}

		public override string ToString ()
		{
			return string.Format ("<" + Name + ">");
		}
	}

	public class EndTagToken : TagToken {
		public EndTagToken (int ch) : base (TokenType.EndTag, ch)
		{
		}

		public override string ToString ()
		{
			return string.Format ("</" + Name + ">");
		}
	}

	public class CommentToken : Token {

		StringBuilder _data;

		public string Data { get { return _data.ToString (); } }

		public CommentToken () : base (TokenType.Comment) {
			_data = new StringBuilder();
		}

		public void AppendData (int ch)
		{
			_data.Append ((char)ch);
		}

		public override string ToString ()
		{
			return string.Format ("<!-- " + Data + " -->");
		}
	}

	public class Token
	{
		public readonly TokenType Type;

		public Token (TokenType type)
		{
			Type = type;
		}

		TokenFlags _flags;

		public void SetFlag (TokenFlags flag, bool on) {
			if (on) { _flags |= flag; }
			else { _flags &= ~flag; }
		}

		public char Character;

		public static Token CharacterToken (int ch)
		{
			return new Token (TokenType.Character) {
				Character = (char)ch
			};
		}

		public static Token EndOfFileToken ()
		{
			return new Token (TokenType.EndOfFile) {
			};
		}

		public override string ToString ()
		{
			return Character.ToString ();
		}
	}
}

