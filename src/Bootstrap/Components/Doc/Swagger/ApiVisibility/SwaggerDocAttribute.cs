using System;

namespace Bootstrap.Components.Doc.Swagger.ApiVisibility
{
    public class SwaggerDocAttribute : Attribute
    {
        public SwaggerDocAttribute(string docName)
        {
            DocName = docName;
        }

        public string DocName { get; }

    }
}