using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.MetaData.Models;

namespace Coevery.Metadata.HtmlControlProviders
{
    public class TextFiledProvider : ControlProviderBase
    {
        public TextFiledProvider(ContentPartFieldDefinition field)
        {
            _field = field;
        }
    }

    public class NumericFieldProvider : ControlProviderBase
    {
            public NumericFieldProvider(ContentPartFieldDefinition field)
        {
            _field = field;
        }
       
    }

    public class DateTimeFieldProvider : ControlProviderBase
    {
            public DateTimeFieldProvider(ContentPartFieldDefinition field)
        {
            _field = field;
        }
       
    }

    public class EnumrationFieldProvider : ControlProviderBase
    {
            public EnumrationFieldProvider(ContentPartFieldDefinition field)
        {
            _field = field;
        }
       
    }

    public class BooleanFieldProvider : ControlProviderBase
    {
        public BooleanFieldProvider(ContentPartFieldDefinition field)
        {
            _field = field;
        }
    }
}