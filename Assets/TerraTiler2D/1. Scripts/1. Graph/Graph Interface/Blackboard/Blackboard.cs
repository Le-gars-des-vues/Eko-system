using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
#if (UNITY_EDITOR)
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
#endif

namespace TerraTiler2D
{
    public class Blackboard
#if (UNITY_EDITOR)
        : UnityEditor.Experimental.GraphView.Blackboard
#endif
    {
        private List<Blackboard_Property_Abstract> properties = new List<Blackboard_Property_Abstract>();

        public Blackboard(GraphView graphView, Vector2 blackboardSize, Vector2 blackboardSpacing, string title = "Properties", string subTitle = "")
        {
#if (UNITY_EDITOR)
            //Remove the button from the header.
            VisualElement blackboardHeader = this.contentContainer.parent.Children().First(x => x.name == "header");
            blackboardHeader.Remove(blackboardHeader.Children().First(x => x.GetType() == typeof(Button)));

            //Create an overlay
            //Give the overlay a header.
            this.title = title;
            this.subTitle = "";

            //Add extra right mouse button menu options
            this.AddManipulator(new ContextualMenuManipulator(BuildBlackboardContextualMenu));

            //Make the overlay scrollable.
            scrollable = true;

            //Keep empty space at the bottom of the blackboard
            contentContainer.style.paddingBottom = 50;

            //When the user tries to rename a value, check if the new name is unique.
            editTextRequested = (blackboard1, element, newValue) =>
            {
                //Store the property name.
                var oldPropertyName = ((BlackboardField)element).text;
                //Check if the name is unique.
                if (properties.Any(x => x.PropertyName == newValue))
                {
                    Glob.GetInstance().DebugString("There is already a property with the name '" + newValue + "'. Please use a different name.", Glob.DebugCategories.Error, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                    return;
                }
                else if (string.IsNullOrEmpty(newValue))
                {
                    Glob.GetInstance().DebugString("Property name cannot be empty. Please use a different name.", Glob.DebugCategories.Error, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                    return;
                }

                //Apply the new name.
                Blackboard_Property_Abstract changedProperty = properties.Find(x => x.PropertyName == oldPropertyName);
                changedProperty.PropertyName = newValue;
                ((BlackboardField)element).text = newValue;

                //If the property name is changed, raise an event to inform listeners
                EventManager.GetInstance().RaiseEvent(new PropertyChangedEvent().Init(changedProperty.GetPropertyData()));
            };

            moveItemRequested = (blackboard1, newIndex, element) =>
            {
                Debug.Log("Move request");
                //Get the old index
                int currentIndex = blackboard1.IndexOf(element);
                //Insert the element at the new index
                blackboard1.hierarchy.Insert(newIndex, element);

                //The old index has been shifted by one if it was later in the list than the newIndex.
                if (currentIndex >= newIndex)
                {
                    currentIndex++;
                }
                //Remove the old element.
                blackboard1.hierarchy.RemoveAt(currentIndex);
            };

            //Position the overlay.
            SetPosition(new Rect(blackboardSpacing.x, blackboardSpacing.y, blackboardSize.x, blackboardSize.y));
#endif

            //Add the overlay to the graph.
            graphView.AddBlackboard(this);
        }

        public List<Blackboard_Property_Abstract> GetProperties()
        {
            return properties;
        }
        public Blackboard_Property_Abstract GetProperty(string name)
        {
            foreach (Blackboard_Property_Abstract property in properties)
            {
                if (property.PropertyName == name)
                {
                    return property;
                }
            }

            return null;
        }
#if (UNITY_EDITOR)
        private Blackboard_Property_Abstract GetProperty(VisualElement element)
        {
            foreach (Blackboard_Property_Abstract property in properties)
            {
                if (property.PropertyElement == element)
                {
                    return property;
                }
            }

            return null;
        }
#endif
        public T GetBlackboardPropertyValue<T>(string name)
        {
            return (GetProperty(name) as Blackboard_Property<T>).GetPropertyValue();
        }

        public void AddPropertyToBlackboard(Blackboard_Property_Abstract property)
        {
            //Generate a unique name for the new property.
            string newPropertyName = property.PropertyName;
            int nameIterator = 1;
            while (properties.Any(x => x.PropertyName == property.PropertyName))
            {
                property.PropertyName = newPropertyName + " (" + nameIterator + ")";
                nameIterator++;
            }

            Type propertyType = property.GetPropertyType();

            //Add the property to the list
            properties.Add(property);

#if (UNITY_EDITOR)
            //Create a visual element to display the property on the screen.
            VisualElement container = new VisualElement();
            container.name = "PropertyContainer";
            //Create a field, and set the text of the field to the property name, and the typeText to the type of the property.
            BlackboardField blackboardField = new BlackboardField { text = property.PropertyName, typeText = propertyType.Name.ToString() };

            colorBlackboardField(blackboardField, propertyType);

            //Add the name field to the container
            container.Add(blackboardField);

            //Show a menu when the field is right clicked.
            container.AddManipulator(new ContextualMenuManipulator(BuildPropertyContextualMenu));

            //Add a value field
            addPropertyValueField(property, container, blackboardField, propertyType);

            //Add the container to the blackboard
            this.Add(container);
            //Make the field droppable.
            //TODO: Does not work currently.
            blackboardField.capabilities &= Capabilities.Droppable;

            //Store the visual element in the blackboard property.
            property.PropertyElement = container;
#endif
        }
        //Convert from abstract to generic type
        public void LoadProperty(PropertyData_Abstract propertyData)
        {
            AddPropertyToBlackboard(propertyData.LoadProperty());  
        }

#if (UNITY_EDITOR)
        private void colorBlackboardField(BlackboardField blackboardField, Type propertyType)
        {
            VisualElement pill = Glob.GetInstance().GetFirstChildVisualElementOfType(blackboardField, typeof(Pill)).ElementAt(0).ElementAt(0);

            Color pillColor = default(Color);

            if (Glob.GetInstance().TypeColors.ContainsKey(propertyType))
            {
                pillColor = Glob.GetInstance().TypeColors[propertyType];
            }

            if (pillColor == null || pillColor == default(Color))
            {
                return;
            }

            pill.style.unityBackgroundImageTintColor = pillColor;

            pill.style.backgroundImage = new StyleBackground(Glob.GetInstance().Property_9Tile_Icon);
            pill.style.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;

            //TODO: Make sure the text is always readable by adding an outline, or by dynamically changing the text color.
            pill.ElementAt(0).ElementAt(0).ElementAt(2).style.color = Color.black;
        }
#endif

#if (UNITY_EDITOR)
        //Add a value field to the blackboard entry, based on the generic type of the property.
        private void addPropertyValueField(Blackboard_Property_Abstract property, VisualElement container, BlackboardField blackboardField, Type type)
        {
            VisualElement allFields = new VisualElement();

            if (type == typeof(int))
            {
                BaseField<int> propertyValueField = new IntegerField("Value:")
                {
                    value = (property as Blackboard_Property<int>).GetPropertyValue()
                } as BaseField<int>;

                propertyValueField.RegisterValueChangedCallback(evt =>
                {
                    (property as Blackboard_Property<int>).SetPropertyValue(evt.newValue, true);
                });

                allFields.Add(propertyValueField);
            }
            else if (type == typeof(float))
            {
                BaseField<float> propertyValueField = new FloatField("Value:")
                {
                    value = (property as Blackboard_Property<float>).GetPropertyValue()
                } as BaseField<float>;

                propertyValueField.RegisterValueChangedCallback(evt =>
                {
                    (property as Blackboard_Property<float>).SetPropertyValue(evt.newValue, true);
                });

                allFields.Add(propertyValueField);

                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).StretchToParentWidth();
                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).style.left = 50;
            }
            else if (type == typeof(bool))
            {
                BaseField<bool> propertyValueField = new Toggle("Value:")
                {
                    value = (property as Blackboard_Property<bool>).GetPropertyValue()
                } as BaseField<bool>;

                propertyValueField.RegisterValueChangedCallback(evt =>
                {
                    (property as Blackboard_Property<bool>).SetPropertyValue(evt.newValue, true);
                });

                allFields.Add(propertyValueField);

                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).StretchToParentWidth();
                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).style.left = 50;
            }
            else if (type == typeof(Vector2))
            {
                BaseField<Vector2> propertyValueField = new Vector2Field()
                {
                    value = (property as Blackboard_Property<Vector2>).GetPropertyValue()
                } as BaseField<Vector2>;

                propertyValueField.RegisterValueChangedCallback(evt =>
                {
                    (property as Blackboard_Property<Vector2>).SetPropertyValue(evt.newValue, true);
                });

                allFields.Add(propertyValueField);
            }
            else if (type == typeof(Vector3))
            {
                BaseField<Vector3> propertyValueField = new Vector3Field()
                {
                    value = (property as Blackboard_Property<Vector3>).GetPropertyValue()
                } as BaseField<Vector3>;

                propertyValueField.RegisterValueChangedCallback(evt =>
                {
                    (property as Blackboard_Property<Vector3>).SetPropertyValue(evt.newValue, true);
                });

                allFields.Add(propertyValueField);
            }
            else if (type == typeof(Vector4))
            {
                BaseField<Vector4> propertyValueField = new Vector4Field()
                {
                    value = (property as Blackboard_Property<Vector4>).GetPropertyValue()
                } as BaseField<Vector4>;

                propertyValueField.RegisterValueChangedCallback(evt =>
                {
                    (property as Blackboard_Property<Vector4>).SetPropertyValue(evt.newValue, true);
                });

                allFields.Add(propertyValueField);
            }
            else if (type == typeof(string))
            {
                BaseField<string> propertyValueField = new TextField("Value:")
                {
                    value = (property as Blackboard_Property_Cloneable<string>).GetPropertyValue()
                } as BaseField<string>;

                propertyValueField.RegisterValueChangedCallback(evt =>
                {
                    (property as Blackboard_Property_Cloneable<string>).SetPropertyValue(evt.newValue, true);
                });

                allFields.Add(propertyValueField);

                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).StretchToParentWidth();
                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).style.left = 50;
            }
            else if (type == typeof(Color))
            {
                BaseField<Color> propertyValueField = new ColorField("Value:")
                {
                    value = (property as Blackboard_Property<Color>).GetPropertyValue()
                } as BaseField<Color>;

                propertyValueField.RegisterValueChangedCallback(evt =>
                {
                    (property as Blackboard_Property<Color>).SetPropertyValue(evt.newValue, true);
                });

                allFields.Add(propertyValueField);

                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).StretchToParentWidth();
                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).style.left = 50;
            }
            else if (type == typeof(Gradient))
            {
                BaseField<Gradient> propertyValueField = new GradientField("Value:")
                {
                    value = (property as Blackboard_Property<Gradient>).GetPropertyValue()
                } as BaseField<Gradient>;

                propertyValueField.RegisterValueChangedCallback(evt =>
                {
                    (property as Blackboard_Property<Gradient>).SetPropertyValue(evt.newValue, true);
                });

                allFields.Add(propertyValueField);

                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).StretchToParentWidth();
                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).style.left = 50;
            }
            else if (type == typeof(Texture2D))
            {
                ObjectField propertyValueField = new ObjectField("Value:")
                {
                    objectType = typeof(Texture2D)
                };

                propertyValueField.value = (property as Blackboard_Property<Texture2D>).GetPropertyValue() as Texture2D;
                propertyValueField.RegisterValueChangedCallback(evt => 
                { 
                    (property as Blackboard_Property<Texture2D>).SetPropertyValue((Texture2D)evt.newValue, true); 
                });

                allFields.Add(propertyValueField);

                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).StretchToParentWidth();
                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).style.left = 50;
                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).style.top = 0;
                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).style.bottom = 0;
            }
            else if (type == typeof(TileBase))
            {
                ObjectField propertyValueField = new ObjectField("Value:")
                {
                    objectType = typeof(TileBase)
                };

                propertyValueField.value = (property as Blackboard_Property<TileBase>).GetPropertyValue() as TileBase;
                propertyValueField.RegisterValueChangedCallback(evt => { (property as Blackboard_Property<TileBase>).SetPropertyValue((TileBase)evt.newValue, true); });

                allFields.Add(propertyValueField);

                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).StretchToParentWidth();
                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).style.left = 50;
                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).style.top = 0;
                propertyValueField.Children().First(x => x.GetType() != typeof(Label)).style.bottom = 0;
            }
            else
            {
                return;
            }

            BlackboardRow blackboardValueRow = new BlackboardRow(blackboardField, allFields);
            container.Add(blackboardValueRow);
        }
#endif

        private void RemovePropertyFromBlackboard(Blackboard_Property_Abstract property)
        {
            EventManager.GetInstance().RaiseEvent(new PropertyDeletedEvent().Init(property.GetPropertyData()));

            //Remove the property from the list
            properties.Remove(property);
#if (UNITY_EDITOR)
            //Remove the visual element from the blackboard
            this.Remove(property.PropertyElement);
#endif
        }
#if (UNITY_EDITOR)
        private void RemovePropertyFromBlackboard(VisualElement propertyField)
        {
            //If this visual element is bound to any property in the list
            if (properties.Any(x => x.PropertyElement == propertyField))
            {
                //Handle the removal of that property.
                RemovePropertyFromBlackboard(properties.First(x => x.PropertyElement == propertyField));
                return;
            }
        }

        private void BuildBlackboardContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("New Int", x => { AddPropertyToBlackboard(new Blackboard_Property<int>("New Integer", 0)); });
            evt.menu.AppendAction("New Float", x=> { AddPropertyToBlackboard(new Blackboard_Property<float>("New Float", 0.0f)); });
            evt.menu.AppendAction("New Bool", x => { AddPropertyToBlackboard(new Blackboard_Property<bool>("New Bool", true)); });
            evt.menu.AppendAction("New Vector2", x=> { AddPropertyToBlackboard(new Blackboard_Property<Vector2>("New Vector2", Vector2.zero)); });
            evt.menu.AppendAction("New Vector3", x => { AddPropertyToBlackboard(new Blackboard_Property<Vector3>("New Vector3", Vector3.zero)); });
            evt.menu.AppendAction("New Vector4", x => { AddPropertyToBlackboard(new Blackboard_Property<Vector4>("New Vector4", Vector4.zero)); });
            evt.menu.AppendAction("New String", x => { AddPropertyToBlackboard(new Blackboard_Property_Cloneable<string>("New String", "")); });
            evt.menu.AppendAction("New Color", x => { AddPropertyToBlackboard(new Blackboard_Property<Color>("New Color", Color.white)); });
            evt.menu.AppendAction("New Gradient", x => { AddPropertyToBlackboard(new Blackboard_Property<Gradient>("New Gradient", new Gradient())); });
            evt.menu.AppendAction("New Texture2D", x => { AddPropertyToBlackboard(new Blackboard_Property<Texture2D>("New Texture2D", null)); });
            evt.menu.AppendAction("New Tile", x => { AddPropertyToBlackboard(new Blackboard_Property<TileBase>("New Tile", null)); });
            evt.menu.AppendAction("New World", x => { AddPropertyToBlackboard(new Blackboard_Property_Cloneable<World>("New World", new World())); });

            evt.StopPropagation();
        }

        private void BuildPropertyContextualMenu(ContextualMenuPopulateEvent evt)
        {
            VisualElement propertyField = GetParentPropertyElement((VisualElement)evt.target);
            Type genericType = GetProperty(propertyField).GetType().GetGenericArguments()[0];

            Glob.NodeTypes nodeType = Glob.NodeTypes.Default;
            if (genericType == typeof(int))
            {
                nodeType = Glob.NodeTypes.IntProperty;
            }
            else if (genericType == typeof(float))
            {
                nodeType = Glob.NodeTypes.FloatProperty;
            }
            else if (genericType == typeof(bool))
            {
                nodeType = Glob.NodeTypes.BoolProperty;
            }
            else if (genericType == typeof(Vector2))
            {
                nodeType = Glob.NodeTypes.Vector2Property;
            }
            else if (genericType == typeof(Vector3))
            {
                nodeType = Glob.NodeTypes.Vector3Property;
            }
            else if (genericType == typeof(Vector4))
            {
                nodeType = Glob.NodeTypes.Vector4Property;
            }
            else if (genericType == typeof(string))
            {
                nodeType = Glob.NodeTypes.StringProperty;
            }
            else if (genericType == typeof(Color))
            {
                nodeType = Glob.NodeTypes.ColorProperty;
            }
            else if (genericType == typeof(Gradient))
            {
                nodeType = Glob.NodeTypes.GradientProperty;
            }
            else if (genericType == typeof(Texture2D))
            {
                nodeType = Glob.NodeTypes.Texture2DProperty;
            }
            else if (genericType == typeof(TileBase))
            {
                nodeType = Glob.NodeTypes.TileProperty;
            }
            else if (genericType == typeof(World))
            {
                nodeType = Glob.NodeTypes.WorldProperty;
            }

            evt.menu.AppendAction("Make Getter node", x => { Graph.Instance.GetGraphView().CreateNode(nodeType, ((GraphView)graphView).GetGraphViewCenter(), GetProperty(propertyField).PropertyName); });
            evt.menu.AppendAction("Make Setter node", x => { Graph.Instance.GetGraphView().CreateNode((Glob.NodeTypes)((int)nodeType+1), ((GraphView)graphView).GetGraphViewCenter(), GetProperty(propertyField).PropertyName); });
            evt.menu.AppendAction("Delete", x => { RemovePropertyFromBlackboard(propertyField); });

            //Prevent the contextual menus of visual elements behind this element from showing.
            evt.StopPropagation();
        }
        private VisualElement GetParentPropertyElement(VisualElement child)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                if (Glob.GetInstance().ContainsVisualElementIterative(properties[i].PropertyElement, child))
                {
                    return properties[i].PropertyElement;
                }
            }

            return null;
        }
#endif

        public void ClearBlackboard()
        {
            while (properties.Count > 0)
            {
                RemovePropertyFromBlackboard(properties[0]);
            }
        }
    }
}