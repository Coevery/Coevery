﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<ShellSettings>" %>
<%@ Import Namespace="Orchard.Environment.Configuration" %>
<% using(Html.BeginFormAntiForgeryPost(Url.Action("disable", new {area = "Orchard.MultiTenancy"}), FormMethod.Post, new {@class = "inline link"})) { %>
<%: Html.HiddenFor(ss => ss.Name) %>
<button type="submit"><%: T("Suspend") %></button><%
   } %>