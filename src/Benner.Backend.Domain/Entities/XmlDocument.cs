using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Benner.Backend.Domain.Common;

namespace Benner.Backend.Domain.Entities
{
    [XmlRoot("Document")]
    public class XmlDocument : BaseEntity
    {
        public XmlDocument()
        {
            Elements = new List<XmlElement>();
            Version = "1.0";
        }

        public XmlDocument(string name, string content) : this()
        {
            ValidateName(name);
            Name = name;
            Content = content;
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlElement("Content")]
        public string Content { get; set; }

        [XmlArray("Elements")]
        [XmlArrayItem("Element")]
        public List<XmlElement> Elements { get; set; }

        public void AddElement(XmlElement element)
        {
            Elements.Add(element);
            SetUpdatedAt();
        }

        public void RemoveElement(Guid elementId)
        {
            var element = Elements.Find(e =>
            {
                var idAttribute = e.GetAttribute("Id");
                return Guid.TryParse(idAttribute, out var id) && id == elementId;
            });

            if (element == null)
                return;

            Elements.Remove(element);
            SetUpdatedAt();
        }

        public void UpdateContent(string newContent)
        {
            Content = newContent ?? string.Empty;
            SetUpdatedAt();
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome do documento é obrigatório", nameof(name));
        }
    }
}