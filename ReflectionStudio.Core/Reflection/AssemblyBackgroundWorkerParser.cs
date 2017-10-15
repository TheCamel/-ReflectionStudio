using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ReflectionStudio.Core.Reflection.Types;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Properties;

namespace ReflectionStudio.Core.Reflection
{
	public class AssemblyBackgroundWorkerParser : BackgroundWorker
	{
		private eParserType _ParserType;

		public AssemblyBackgroundWorkerParser(eParserType typ)
		{
			_ParserType = typ;

			RunWorkerCompleted += new RunWorkerCompletedEventHandler(ParsingWorkCompleted);
		}

		public void Parse(string filePath)
		{
			//EventDispatcher.Instance.RaiseStatus(Resources.CORE_PARSING_START, StatusEventType.StartProgress);
			DoWork += new DoWorkEventHandler(DoParsingWork);
			RunWorkerAsync(filePath);
		}

		public void Refresh(ObservableCollection<NetAssembly> assList)
		{
			//EventDispatcher.Instance.RaiseStatus(Resources.CORE_PARSING_START, StatusEventType.StartProgress);
			DoWork += new DoWorkEventHandler(DoRefreshParsingWork);
			RunWorkerAsync(assList);
		}

		private void DoParsingWork(object sender, DoWorkEventArgs e)
		{
			Tracer.Verbose("AssemblyBackgroundWorkerParser:DoParsingWork", "START");
			try
			{
				IAssemblyParser parser = AssemblyParserFactory.GetParser(_ParserType);
				e.Result = parser.LoadAssemblyRecursively((string)e.Argument);
			}
			catch (Exception err)
			{
				Tracer.Error("AssemblyBackgroundWorkerParser.DoParsingWork", err);
			}
			finally
			{
				Tracer.Verbose("AssemblyBackgroundWorkerParser:DoParsingWork", "END");

				//be sure progress is stoped
				EventDispatcher.Instance.RaiseStatus("", StatusEventType.StopProgress);
			}
		}

		private void DoRefreshParsingWork(object sender, DoWorkEventArgs e)
		{
			Tracer.Verbose("AssemblyBackgroundWorkerParser:DoRefreshParsingWork", "START");

			try
			{
				IAssemblyParser parser = AssemblyParserFactory.GetParser(_ParserType);

				foreach (NetAssembly typ in ((ObservableCollection<NetAssembly>)e.Argument))
					parser.RefreshAssembly(typ);
			}
			catch (Exception err)
			{
				Tracer.Error("AssemblyBackgroundWorkerParser.DoRefreshParsingWork", err);
			}
			finally
			{
				Tracer.Verbose("AssemblyBackgroundWorkerParser:DoRefreshParsingWork", "END");

				//be sure progress is stoped
				EventDispatcher.Instance.RaiseStatus("", StatusEventType.StopProgress);
			}
		}

		private void ParsingWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Tracer.Verbose("AssemblyBackgroundWorkerParser:ParsingWorkCompleted", "START");

			try
			{
				if (e.Error != null)
				{
					System.Media.SystemSounds.Hand.Play();

					// First, handle the case where an exception was thrown.
					EventDispatcher.Instance.RaiseStatus(Resources.CORE_PARSING_ERROR, StatusEventType.StopProgress);
				}
				else if (e.Cancelled)
				{
					// Next, handle the case where the user canceled the operation.
					EventDispatcher.Instance.RaiseStatus(Resources.CORE_PARSING_CANCELED, StatusEventType.StopProgress);
				}
				else if (e.Result != null)
				{
					System.Media.SystemSounds.Exclamation.Play();

					// Finally, handle the case where the operation succeeded.
					EventDispatcher.Instance.RaiseStatus(Resources.CORE_PARSING_FINISH, StatusEventType.StopProgress);
				}
			}
			catch (Exception err)
			{
				Tracer.Error("AssemblyBackgroundWorkerParser.ParsingWorkCompleted", err);
			}
			finally
			{
				Tracer.Verbose("AssemblyBackgroundWorkerParser:ParsingWorkCompleted", "END");

				//be sure progress is stopped
				EventDispatcher.Instance.RaiseStatus("", StatusEventType.StopProgress);
			}
		}
	}
}
