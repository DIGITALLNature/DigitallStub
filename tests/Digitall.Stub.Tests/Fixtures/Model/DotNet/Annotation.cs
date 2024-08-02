using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query; 
using AttributeCollection = Microsoft.Xrm.Sdk.AttributeCollection;

// ReSharper disable All
namespace Digitall.Stub.Tests.Fixtures
{
	/// <inheritdoc />
	/// <summary>
	/// Note that is attached to one or more objects, including other notes.
	/// </summary>
	[DataContractAttribute()]
	[EntityLogicalNameAttribute("annotation")]
	[System.CodeDom.Compiler.GeneratedCode("dgtp", "2023")]
    [ExcludeFromCodeCoverage]
	public partial class Annotation : Entity, INotifyPropertyChanging, INotifyPropertyChanged
    {
	    #region ctor
		[DebuggerNonUserCode]
		public Annotation() : this(false)
        {
        }

        [DebuggerNonUserCode]
		public Annotation(bool trackChanges = false) : base(EntityLogicalName)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public Annotation(Guid id, bool trackChanges = false) : base(EntityLogicalName,id)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public Annotation(KeyAttributeCollection keyAttributes, bool trackChanges = false) : base(EntityLogicalName,keyAttributes)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public Annotation(string keyName, object keyValue, bool trackChanges = false) : base(EntityLogicalName, keyName, keyValue)
        {
			_trackChanges = trackChanges;
        }
        #endregion

		#region fields
        private readonly bool _trackChanges;
        private readonly Lazy<HashSet<string>> _changedProperties = new Lazy<HashSet<string>>();
        #endregion

        #region consts
        public const string EntityLogicalName = "annotation";
        public const string PrimaryNameAttribute = "subject";
        public const int EntityTypeCode = 5;
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        [DebuggerNonUserCode]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (_trackChanges)
            {
                _changedProperties.Value.Add(propertyName);
            }
        }

        [DebuggerNonUserCode]
		private void OnPropertyChanging([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanging != null) PropertyChanging.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        #endregion

		#region Attributes
		[AttributeLogicalNameAttribute("annotationid")]
		public new System.Guid Id
		{
		    [DebuggerNonUserCode]
			get
			{
				return base.Id;
			}
            [DebuggerNonUserCode]
			set
			{
				AnnotationId = value;
			}
		}

		/// <summary>
		/// Unique identifier of the note.
		/// </summary>
		[AttributeLogicalName("annotationid")]
        public Guid? AnnotationId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("annotationid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(AnnotationId));
                SetAttributeValue("annotationid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
                OnPropertyChanged(nameof(AnnotationId));
            }
        }

		/// <summary>
		/// Unique identifier of the user who created the note.
		/// </summary>
		[AttributeLogicalName("createdby")]
        public EntityReference CreatedBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("createdby");
            }
        }

		/// <summary>
		/// Date and time when the note was created.
		/// </summary>
		[AttributeLogicalName("createdon")]
        public DateTime? CreatedOn
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<DateTime?>("createdon");
            }
        }

		/// <summary>
		/// Unique identifier of the delegate user who created the annotation.
		/// </summary>
		[AttributeLogicalName("createdonbehalfby")]
        public EntityReference CreatedOnBehalfBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("createdonbehalfby");
            }
        }

		/// <summary>
		/// Contents of the note's attachment.
		/// </summary>
		[AttributeLogicalName("documentbody")]
        public string DocumentBody
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("documentbody");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DocumentBody));
                SetAttributeValue("documentbody", value);
                OnPropertyChanged(nameof(DocumentBody));
            }
        }

		/// <summary>
		/// File name of the note.
		/// </summary>
		[AttributeLogicalName("filename")]
        public string FileName
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("filename");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(FileName));
                SetAttributeValue("filename", value);
                OnPropertyChanged(nameof(FileName));
            }
        }

		/// <summary>
		/// File size of the note.
		/// </summary>
		[AttributeLogicalName("filesize")]
        public int? FileSize
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("filesize");
            }
        }

		/// <summary>
		/// Unique identifier of the data import or data migration that created this record.
		/// </summary>
		[AttributeLogicalName("importsequencenumber")]
        public int? ImportSequenceNumber
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("importsequencenumber");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ImportSequenceNumber));
                SetAttributeValue("importsequencenumber", value);
                OnPropertyChanged(nameof(ImportSequenceNumber));
            }
        }

		/// <summary>
		/// Specifies whether the note is an attachment.
		/// </summary>
		[AttributeLogicalName("isdocument")]
        public bool? IsDocument
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("isdocument");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IsDocument));
                SetAttributeValue("isdocument", value);
                OnPropertyChanged(nameof(IsDocument));
            }
        }

		/// <summary>
		/// Language identifier for the note.
		/// </summary>
		[AttributeLogicalName("langid")]
        public string LangId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("langid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(LangId));
                SetAttributeValue("langid", value);
                OnPropertyChanged(nameof(LangId));
            }
        }

		/// <summary>
		/// MIME type of the note's attachment.
		/// </summary>
		[AttributeLogicalName("mimetype")]
        public string MimeType
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("mimetype");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(MimeType));
                SetAttributeValue("mimetype", value);
                OnPropertyChanged(nameof(MimeType));
            }
        }

		/// <summary>
		/// Unique identifier of the user who last modified the note.
		/// </summary>
		[AttributeLogicalName("modifiedby")]
        public EntityReference ModifiedBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("modifiedby");
            }
        }

		/// <summary>
		/// Date and time when the note was last modified.
		/// </summary>
		[AttributeLogicalName("modifiedon")]
        public DateTime? ModifiedOn
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<DateTime?>("modifiedon");
            }
        }

		/// <summary>
		/// Unique identifier of the delegate user who last modified the annotation.
		/// </summary>
		[AttributeLogicalName("modifiedonbehalfby")]
        public EntityReference ModifiedOnBehalfBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("modifiedonbehalfby");
            }
        }

		
		[AttributeLogicalName("msft_datastate")]
        public OptionSetValue MsftDataState
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("msft_datastate");
            }
        }

		/// <summary>
		/// Text of the note.
		/// </summary>
		[AttributeLogicalName("notetext")]
        public string NoteText
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("notetext");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(NoteText));
                SetAttributeValue("notetext", value);
                OnPropertyChanged(nameof(NoteText));
            }
        }

		/// <summary>
		/// Unique identifier of the object with which the note is associated.
		/// </summary>
		[AttributeLogicalName("objectid")]
        public EntityReference ObjectId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("objectid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ObjectId));
                SetAttributeValue("objectid", value);
                OnPropertyChanged(nameof(ObjectId));
            }
        }

		/// <summary>
		/// Type of entity with which the note is associated.
		/// </summary>
		[AttributeLogicalName("objecttypecode")]
        public string ObjectTypeCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("objecttypecode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ObjectTypeCode));
                SetAttributeValue("objecttypecode", value);
                OnPropertyChanged(nameof(ObjectTypeCode));
            }
        }

		/// <summary>
		/// Date and time that the record was migrated.
		/// </summary>
		[AttributeLogicalName("overriddencreatedon")]
        public DateTime? OverriddenCreatedOn
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<DateTime?>("overriddencreatedon");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(OverriddenCreatedOn));
                SetAttributeValue("overriddencreatedon", value);
                OnPropertyChanged(nameof(OverriddenCreatedOn));
            }
        }

		/// <summary>
		/// Unique identifier of the user or team who owns the note.
		/// </summary>
		[AttributeLogicalName("ownerid")]
        public EntityReference OwnerId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("ownerid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(OwnerId));
                SetAttributeValue("ownerid", value);
                OnPropertyChanged(nameof(OwnerId));
            }
        }

		/// <summary>
		/// Unique identifier of the business unit that owns the note.
		/// </summary>
		[AttributeLogicalName("owningbusinessunit")]
        public EntityReference OwningBusinessUnit
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("owningbusinessunit");
            }
        }

		/// <summary>
		/// Unique identifier of the team who owns the note.
		/// </summary>
		[AttributeLogicalName("owningteam")]
        public EntityReference OwningTeam
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("owningteam");
            }
        }

		/// <summary>
		/// Unique identifier of the user who owns the note.
		/// </summary>
		[AttributeLogicalName("owninguser")]
        public EntityReference OwningUser
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("owninguser");
            }
        }

		/// <summary>
		/// Prefix of the file pointer in blob storage.
		/// </summary>
		[AttributeLogicalName("prefix")]
        public string Prefix
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("prefix");
            }
        }

		/// <summary>
		/// workflow step id associated with the note.
		/// </summary>
		[AttributeLogicalName("stepid")]
        public string StepId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("stepid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(StepId));
                SetAttributeValue("stepid", value);
                OnPropertyChanged(nameof(StepId));
            }
        }

		/// <summary>
		/// Subject associated with the note.
		/// </summary>
		[AttributeLogicalName("subject")]
        public string Subject
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("subject");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(Subject));
                SetAttributeValue("subject", value);
                OnPropertyChanged(nameof(Subject));
            }
        }

		/// <summary>
		/// Version number of the note.
		/// </summary>
		[AttributeLogicalName("versionnumber")]
        public long? VersionNumber
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<long?>("versionnumber");
            }
        }


		#endregion

		#region NavigationProperties
		#endregion

		#region Options
		public static class Options
		{
                public struct IsDocument
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
			    public struct MsftDataState
                {
					public const int Default = 0;
					public const int Retain = 1;
                }
		}
		#endregion

		#region LogicalNames
		public static class LogicalNames
		{
				public const string AnnotationId = "annotationid";
				public const string CreatedBy = "createdby";
				public const string CreatedOn = "createdon";
				public const string CreatedOnBehalfBy = "createdonbehalfby";
				public const string DocumentBody = "documentbody";
				public const string FileName = "filename";
				public const string FileSize = "filesize";
				public const string ImportSequenceNumber = "importsequencenumber";
				public const string IsDocument = "isdocument";
				public const string LangId = "langid";
				public const string MimeType = "mimetype";
				public const string ModifiedBy = "modifiedby";
				public const string ModifiedOn = "modifiedon";
				public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
				public const string MsftDataState = "msft_datastate";
				public const string NoteText = "notetext";
				public const string ObjectId = "objectid";
				public const string ObjectTypeCode = "objecttypecode";
				public const string OverriddenCreatedOn = "overriddencreatedon";
				public const string OwnerId = "ownerid";
				public const string OwningBusinessUnit = "owningbusinessunit";
				public const string OwningTeam = "owningteam";
				public const string OwningUser = "owninguser";
				public const string Prefix = "prefix";
				public const string StepId = "stepid";
				public const string Subject = "subject";
				public const string VersionNumber = "versionnumber";
		}
		#endregion

		#region Relations
        public static class Relations
        {
            public static class OneToMany
            {
				public const string AnnotationAsyncOperations = "Annotation_AsyncOperations";
				public const string AnnotationBulkDeleteFailures = "Annotation_BulkDeleteFailures";
				public const string AnnotationProcessSessions = "Annotation_ProcessSessions";
				public const string AnnotationSyncErrors = "Annotation_SyncErrors";
				public const string UserentityinstancedataAnnotation = "userentityinstancedata_annotation";
            }

            public static class ManyToOne
            {
				public const string AccountAnnotation = "Account_Annotation";
				public const string AdxInvitationAnnotations = "adx_invitation_Annotations";
				public const string AdxInviteredemptionAnnotations = "adx_inviteredemption_Annotations";
				public const string AdxPortalcommentAnnotations = "adx_portalcomment_Annotations";
				public const string AnnotationOwningUser = "annotation_owning_user";
				public const string AppointmentAnnotation = "Appointment_Annotation";
				public const string BusinessUnitAnnotations = "business_unit_annotations";
				public const string CalendarAnnotation = "Calendar_Annotation";
				public const string ChannelaccessprofileAnnotations = "channelaccessprofile_Annotations";
				public const string ChatAnnotations = "chat_Annotations";
				public const string ContactAnnotation = "Contact_Annotation";
				public const string ConvertRuleAnnotation = "ConvertRule_Annotation";
				public const string DuplicateRuleAnnotation = "DuplicateRule_Annotation";
				public const string EmailAnnotation = "Email_Annotation";
				public const string EmailServerProfileAnnotation = "EmailServerProfile_Annotation";
				public const string FaxAnnotation = "Fax_Annotation";
				public const string GoalAnnotation = "Goal_Annotation";
				public const string KbArticleAnnotation = "KbArticle_Annotation";
				public const string KnowledgearticleAnnotations = "knowledgearticle_Annotations";
				public const string KnowledgeBaseRecordAnnotations = "KnowledgeBaseRecord_Annotations";
				public const string LetterAnnotation = "Letter_Annotation";
				public const string LkAnnotationbaseCreatedby = "lk_annotationbase_createdby";
				public const string LkAnnotationbaseCreatedonbehalfby = "lk_annotationbase_createdonbehalfby";
				public const string LkAnnotationbaseModifiedby = "lk_annotationbase_modifiedby";
				public const string LkAnnotationbaseModifiedonbehalfby = "lk_annotationbase_modifiedonbehalfby";
				public const string MailboxAnnotation = "Mailbox_Annotation";
				public const string MsdynAifptrainingdocumentAnnotations = "msdyn_aifptrainingdocument_Annotations";
				public const string MsdynAimodelAnnotations = "msdyn_aimodel_Annotations";
				public const string MsdynAiodimageAnnotations = "msdyn_aiodimage_Annotations";
				public const string MsfpAlertAnnotations = "msfp_alert_Annotations";
				public const string MsfpQuestionAnnotations = "msfp_question_Annotations";
				public const string MsfpSurveyinviteAnnotations = "msfp_surveyinvite_Annotations";
				public const string MsfpSurveyresponseAnnotations = "msfp_surveyresponse_Annotations";
				public const string MspcatCatalogsubmissionfilesAnnotations = "mspcat_catalogsubmissionfiles_Annotations";
				public const string OwnerAnnotations = "owner_annotations";
				public const string PhoneCallAnnotation = "PhoneCall_Annotation";
				public const string ProfileruleAnnotations = "profilerule_Annotations";
				public const string ProfileruleitemAnnotations = "profileruleitem_Annotations";
				public const string RecurringAppointmentMasterAnnotation = "RecurringAppointmentMaster_Annotation";
				public const string RoutingruleAnnotation = "routingrule_Annotation";
				public const string RoutingruleitemAnnotation = "routingruleitem_Annotation";
				public const string SharePointDocumentAnnotation = "SharePointDocument_Annotation";
				public const string SlaAnnotation = "sla_Annotation";
				public const string SocialActivityAnnotation = "SocialActivity_Annotation";
				public const string TaskAnnotation = "Task_Annotation";
				public const string TeamAnnotations = "team_annotations";
				public const string WorkflowAnnotation = "Workflow_Annotation";
            }

            public static class ManyToMany
            {
            }
        }

        #endregion

		#region Methods
        public EntityReference ToNamedEntityReference()
        {
            var reference = ToEntityReference();
            reference.Name = GetAttributeValue<string>(PrimaryNameAttribute);

            return reference;
        }
        public static Annotation Retrieve(IOrganizationService service, Guid id)
        {
            return Retrieve(service,id, new ColumnSet(true));
        }

        public static Annotation Retrieve(IOrganizationService service, Guid id, ColumnSet columnSet)
        {
            return service.Retrieve("annotation", id, columnSet).ToEntity<Annotation>();
        }

        public Annotation GetChangedEntity()
        {
            if (_trackChanges)
            {
                var attr = new AttributeCollection();
                foreach (var attrName in _changedProperties.Value.Select(changedProperty => ((AttributeLogicalNameAttribute) GetType().GetProperty(changedProperty).GetCustomAttribute(typeof (AttributeLogicalNameAttribute))).LogicalName).Where(attrName => Contains(attrName)))
                {
                    attr.Add(attrName,this[attrName]);
                }
                return new  Annotation(Id) {Attributes = attr };
            }
            return this;
        }
        #endregion
	}

	#region Context
	public partial class DataContext
	{
		public IQueryable<Annotation> AnnotationSet
		{
			get
			{
				return CreateQuery<Annotation>();
			}
		}
	}
	#endregion
}
