using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class GraphOutput
    {
        private List<Blackboard_Property_Abstract> OutputProperties = new List<Blackboard_Property_Abstract>();

        public GraphOutput(List<Blackboard_Property_Abstract> properties)
        {
            OutputProperties = properties;
        }

        /// <summary>
        /// Get the value of a processed graph property.
        /// </summary>
        public T GetProperty<T>(string name)
        {
            foreach (Blackboard_Property_Abstract property in OutputProperties)
            {
                if (property.PropertyName == name)
                {
                    return (property as Blackboard_Property<T>).GetPropertyValue();
                }
            }

            Glob.GetInstance().DebugString("Property with name '" + name + "' does not exist in GraphOutput. Check if you spelled the property name correctly (case-sensitive).", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
            return default(T);
        }

        /// <summary>
        /// Get the value of a processed graph property as an object.
        /// </summary>
        public object GetPropertyAsObject(string name)
        {
            foreach (Blackboard_Property_Abstract property in OutputProperties)
            {
                if (property.PropertyName == name)
                {
                    return property.GetPropertyValueAsObject();
                }
            }

            Glob.GetInstance().DebugString("Property with name '" + name + "' does not exist in GraphOutput. Check if you spelled the property name correctly (case-sensitive).", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
            return null;
        }

        /// <summary>
        /// Get the values of all processed graph properties.
        /// </summary>
        public Dictionary<string, object> GetAllPropertiesAsObject()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            List<string> names = GetAllPropertyNames();
            foreach (string name in names)
            {
                properties.Add(name, GetPropertyAsObject(name));
            }

            return properties;
        }

        /// <summary>
        /// Get the names of all processed graph properties.
        /// </summary>
        public List<string> GetAllPropertyNames()
        {
            List<string> names = new List<string>();
            foreach (Blackboard_Property_Abstract property in OutputProperties)
            {
                names.Add(property.PropertyName);
            }

            return names;
        }
    }
}
