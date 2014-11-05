using MySensors.Controllers.Automation;
using SQLite;
using System;

namespace MySensors.Controllers.Data
{
    class AutomationModuleDto
    {
        [PrimaryKey]
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Script { get; set; }
        public string View { get; set; }

        public static AutomationModuleDto FromModel(AutomationModule item)
        {
            if (item == null)
                return null;

            return new AutomationModuleDto()
            {
                ID = item.ID.ToString(),
                Name = item.Name,
                Description = item.Description,
                Script = item.Script,
                View = item.View
            };
        }
        public AutomationModule ToModel()
        {
            return new AutomationModule(Guid.Parse(ID), Name, Description, Script, View);
        }
    }
}
