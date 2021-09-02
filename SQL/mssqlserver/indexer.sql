create index smf_createtime_index on  [dbo].[t_instance](CreateTime)
create index smf_instanceID_index on  [dbo].[t_node](InstanceID)
create index smf_instanceID_index on  [dbo].[t_link](InstanceID)
create index smf_relationshipid_index on  [dbo].[t_link](RelationshipID)
create index smf_instanceID_index on  [dbo].[t_group](InstanceID)
create index smf_relationshipid_index on  [dbo].[t_group](RelationshipID)
create index smf_instanceID_index on  [dbo].[t_transition](InstanceID)
create index smf_relationshipid_index on  [dbo].[t_transition](RelationshipID)
create index smf_instanceID_index on  [dbo].[t_process](InstanceID)
create index smf_relationshipid_index on  [dbo].[t_process](RelationshipID)
create index smf_instanceID_index on  [dbo].[t_action](InstanceID)
create index smf_relationshipid_index on  [dbo].[t_action](RelationshipID)
create index smf_instanceID_index on  [dbo].[t_actor](InstanceID)
create index smf_relationshipid_index on  [dbo].[t_actor](RelationshipID)
create index smf_instanceID_index on [dbo].[t_bridge](InstanceID)
create index smf_key_index on  [dbo].[t_bridge]([Key])
create index smf_createtime_index on  [dbo].[t_bridge](CreateTime)
create index smf_instanceID_index on  [dbo].[t_carbon](InstanceID)
create index smf_relationshipid_index on  [dbo].[t_carbon](RelationshipID)
create index smf_instanceID_index on  [dbo].[t_carbonCopy](InstanceID)
create index smf_nodeid_index on  [dbo].[t_carbonCopy](NodeID)
create index smf_instanceID_index on  [dbo].[t_command](InstanceID)
create index smf_relationshipid_index on  [dbo].[t_command](RelationshipID)



create index smf_instanceID_index on [dbo].[t_pending](InstanceID)
create index smf_instanceID_index on [dbo].[t_organization](InstanceID)
create index smf_instanceID_index on [dbo].[t_record](InstanceID)
create index smf_instanceID_index on [dbo].[t_rule](InstanceID)
create index smf_categoryCode_index on [dbo].[t_script](CategoryCode)
create index smf_categoryCode_index on [dbo].[t_structure](CategoryCode)
create index smf_createtime_index on  [dbo].[t_structure](CreateTime)