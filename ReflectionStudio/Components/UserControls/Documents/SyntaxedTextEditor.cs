using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit;
using System.Windows;
using ICSharpCode.AvalonEdit.Highlighting;
using System.IO;
using System.Xml;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// Base class for customizing TextEditor from ICSharpCode.AvalonEdit with SQL or template highlighting
	/// </summary>
	internal class SyntaxedTextEditor: TextEditor
	{
		#region --------------------DEPENDENCY PROPERTIES--------------------

		/// <summary>
		/// Two available syntax mode
		/// </summary>
		public enum LocalSyntaxCode { SQL, TEMPLATE };

		/// <summary>
		/// LocalSyntax DependencyProperty
		/// </summary>
		public static readonly DependencyProperty LocalSyntaxDescriptionProperty =
			   DependencyProperty.Register("LocalSyntax", typeof(LocalSyntaxCode), typeof(SyntaxedTextEditor));

		/// <summary>
		/// LocalSyntax property
		/// </summary>
		public LocalSyntaxCode LocalSyntax
		{
			get { return (LocalSyntaxCode)GetValue(LocalSyntaxDescriptionProperty); }
			set { SetValue(LocalSyntaxDescriptionProperty, value); }
		}

		#endregion

		/// <summary>
		/// Load the syntax 
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			LoadHighlighting();
		}

		/// <summary>
		/// Load the syntax based on the LocalSyntax property
		/// </summary>
		private void LoadHighlighting()
		{
			if( LocalSyntax == LocalSyntaxCode.TEMPLATE )
				this.SyntaxHighlighting = CreateAndRegisterHighlightingDefinition("ReflectionStudio.Resources.Embedded.CSharpTemplate.xshd", "C# Template", ".cs");
			else
				if( LocalSyntax == LocalSyntaxCode.SQL )
					this.SyntaxHighlighting = CreateAndRegisterHighlightingDefinition("ReflectionStudio.Resources.Embedded.SQL.xshd", "SQL", ".sql");
		}

		/// <summary>
		/// Load a syntax file into the HighlightingManager
		/// </summary>
		/// <param name="res"></param>
		/// <param name="type"></param>
		/// <param name="extension"></param>
		/// <returns></returns>
		private IHighlightingDefinition CreateAndRegisterHighlightingDefinition(string res, string type, string extension)
		{
			IHighlightingDefinition customHighlighting = null;

			Tracer.Verbose("SyntaxedTextEditor:CreateAndRegisterHighlightingDefinition", "START");
			try
			{
				// Load our custom highlighting definition
				using (Stream s = Application.ResourceAssembly.GetManifestResourceStream(res))
				{
					if (s == null)
						throw new InvalidOperationException("Could not find embedded resource");

					using (XmlReader reader = new XmlTextReader(s))
					{
						customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
					}
				}

				// and register it in the HighlightingManager
				HighlightingManager.Instance.RegisterHighlighting(type, new string[] { extension }, customHighlighting);
			}
			catch (Exception all)
			{
				Tracer.Error("SyntaxedTextEditor.CreateAndRegisterHighlightingDefinition", all);
				return null;
			}

			Tracer.Verbose("SyntaxedTextEditor:CreateAndRegisterHighlightingDefinition", "END");
			
			return customHighlighting;
		}
	}
}
