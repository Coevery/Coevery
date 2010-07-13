﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<Orchard.Indexing.Settings.TypeIndexing>" %>
    <fieldset>
        <%:Html.EditorFor(m=>m.Included) %>
        <label for="<%:Html.FieldIdFor(m => m.Included) %>" class="forcheckbox"><%:T("Index this content type for search") %></label><%:
        Html.ValidationMessageFor(m=>m.Included) %>
    </fieldset>