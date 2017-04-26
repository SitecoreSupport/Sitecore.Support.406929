namespace Sitecore.Support.ContentSearch.Client.Forms
{
  using Sitecore;
  using Sitecore.Configuration;
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.Client.Forms;
  using Sitecore.ContentSearch.Maintenance;
  using Sitecore.Text;
  using Sitecore.Web.UI.HtmlControls;
  using Sitecore.Web.UI.Sheer;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;

  public class IndexingManagerWizard : Sitecore.ContentSearch.Client.Forms.IndexingManagerWizard
  {
    private static readonly Regex DeniedIdSymbolsRegex = new Regex("[^a-z0-9]", RegexOptions.Compiled);

    private string GetHtmlUniqueId(Handle handle) =>
        ("Msg" + DeniedIdSymbolsRegex.Replace(handle.ToString().ToLowerInvariant(), string.Empty));

    protected override void StartRebuilding()
    {
      ListString selectedIndexNames = this.GetSelectedIndexNames();
      Registry.SetString("/Current_User/Rebuild Search Index/Selected", selectedIndexNames.ToString());
      List<Handle> list = (from index in selectedIndexNames.ToArray<string>().Select<string, ISearchIndex>(new Func<string, ISearchIndex>(ContentSearchManager.GetIndex)) select IndexCustodian.FullRebuild(index, true).Handle).ToList<Handle>();
      base.JobHandle = string.Join("|", (IEnumerable<string>)(from h in list select h.ToString()));
      StringBuilder builder = new StringBuilder();
      foreach (Handle handle in list)
      {
        string htmlUniqueId = this.GetHtmlUniqueId(handle);
        builder.Append("<span id=\"").Append(htmlUniqueId).Append("\"></span>");
      }
      SheerResponse.SetInnerHtml("Status", builder.ToString());
      SheerResponse.Timer("CheckStatus", Settings.Publishing.PublishDialogPollingInterval);
    }
  }
}
