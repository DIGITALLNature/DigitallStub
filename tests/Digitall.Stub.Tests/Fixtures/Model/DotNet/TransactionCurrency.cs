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
	/// Currency in which a financial transaction is carried out.
	/// </summary>
	[DataContractAttribute()]
	[EntityLogicalNameAttribute("transactioncurrency")]
	[System.CodeDom.Compiler.GeneratedCode("dgtp", "2023")]
    [ExcludeFromCodeCoverage]
	public partial class TransactionCurrency : Entity, INotifyPropertyChanging, INotifyPropertyChanged
    {
	    #region ctor
		[DebuggerNonUserCode]
		public TransactionCurrency() : this(false)
        {
        }

        [DebuggerNonUserCode]
		public TransactionCurrency(bool trackChanges = false) : base(EntityLogicalName)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public TransactionCurrency(Guid id, bool trackChanges = false) : base(EntityLogicalName,id)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public TransactionCurrency(KeyAttributeCollection keyAttributes, bool trackChanges = false) : base(EntityLogicalName,keyAttributes)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public TransactionCurrency(string keyName, object keyValue, bool trackChanges = false) : base(EntityLogicalName, keyName, keyValue)
        {
			_trackChanges = trackChanges;
        }
        #endregion

		#region fields
        private readonly bool _trackChanges;
        private readonly Lazy<HashSet<string>> _changedProperties = new Lazy<HashSet<string>>();
        #endregion

        #region consts
        public const string EntityLogicalName = "transactioncurrency";
        public const string PrimaryNameAttribute = "currencyname";
        public const int EntityTypeCode = 9105;
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
		[AttributeLogicalNameAttribute("transactioncurrencyid")]
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
				TransactionCurrencyId = value;
			}
		}

		/// <summary>
		/// Unique identifier of the transaction currency.
		/// </summary>
		[AttributeLogicalName("transactioncurrencyid")]
        public Guid? TransactionCurrencyId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("transactioncurrencyid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TransactionCurrencyId));
                SetAttributeValue("transactioncurrencyid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
                OnPropertyChanged(nameof(TransactionCurrencyId));
            }
        }

		/// <summary>
		/// Unique identifier of the user who created the transaction currency.
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
		/// Date and time when the transaction currency was created.
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
		/// Unique identifier of the delegate user who created the transactioncurrency.
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
		/// Name of the transaction currency.
		/// </summary>
		[AttributeLogicalName("currencyname")]
        public string CurrencyName
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("currencyname");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(CurrencyName));
                SetAttributeValue("currencyname", value);
                OnPropertyChanged(nameof(CurrencyName));
            }
        }

		/// <summary>
		/// Number of decimal places that can be used for currency.
		/// </summary>
		[AttributeLogicalName("currencyprecision")]
        public int? CurrencyPrecision
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("currencyprecision");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(CurrencyPrecision));
                SetAttributeValue("currencyprecision", value);
                OnPropertyChanged(nameof(CurrencyPrecision));
            }
        }

		/// <summary>
		/// Symbol for the transaction currency.
		/// </summary>
		[AttributeLogicalName("currencysymbol")]
        public string CurrencySymbol
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("currencysymbol");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(CurrencySymbol));
                SetAttributeValue("currencysymbol", value);
                OnPropertyChanged(nameof(CurrencySymbol));
            }
        }

		/// <summary>
		/// The default image for the entity.
		/// </summary>
		[AttributeLogicalName("entityimage")]
        public byte[] EntityImage
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<byte[]>("entityimage");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(EntityImage));
                SetAttributeValue("entityimage", value);
                OnPropertyChanged(nameof(EntityImage));
            }
        }

		
		[AttributeLogicalName("entityimage_timestamp")]
        public long? EntityImageTimestamp
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<long?>("entityimage_timestamp");
            }
        }

		
		[AttributeLogicalName("entityimage_url")]
        public string EntityImageURL
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("entityimage_url");
            }
        }

		/// <summary>
		/// For internal use only.
		/// </summary>
		[AttributeLogicalName("entityimageid")]
        public Guid? EntityImageId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("entityimageid");
            }
        }

		/// <summary>
		/// Exchange rate between the transaction currency and the base currency.
		/// </summary>
		[AttributeLogicalName("exchangerate")]
        public decimal? ExchangeRate
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<decimal?>("exchangerate");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ExchangeRate));
                SetAttributeValue("exchangerate", value);
                OnPropertyChanged(nameof(ExchangeRate));
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
		/// ISO currency code for the transaction currency.
		/// </summary>
		[AttributeLogicalName("isocurrencycode")]
        public string ISOCurrencyCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("isocurrencycode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ISOCurrencyCode));
                SetAttributeValue("isocurrencycode", value);
                OnPropertyChanged(nameof(ISOCurrencyCode));
            }
        }

		/// <summary>
		/// Unique identifier of the user who last modified the transaction currency.
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
		/// Date and time when the transaction currency was last modified.
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
		/// Unique identifier of the delegate user who last modified the transactioncurrency.
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

		/// <summary>
		/// Unique identifier of the organization associated with the transaction currency.
		/// </summary>
		[AttributeLogicalName("organizationid")]
        public EntityReference OrganizationId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("organizationid");
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
		/// Status of the transaction currency.
		/// </summary>
		[AttributeLogicalName("statecode")]
        public OptionSetValue StateCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("statecode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(StateCode));
                SetAttributeValue("statecode", value);
                OnPropertyChanged(nameof(StateCode));
            }
        }

		/// <summary>
		/// Reason for the status of the transaction currency.
		/// </summary>
		[AttributeLogicalName("statuscode")]
        public OptionSetValue StatusCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("statuscode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(StatusCode));
                SetAttributeValue("statuscode", value);
                OnPropertyChanged(nameof(StatusCode));
            }
        }

		/// <summary>
		/// Version number of the transaction currency.
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
		/// <summary>
		/// 1:N transactioncurrency_account
		/// </summary>	
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("transactioncurrency_account")]
		public System.Collections.Generic.IEnumerable<Account> TransactioncurrencyAccount
		{
			[DebuggerNonUserCode]
			get
			{
				return this.GetRelatedEntities<Account>("transactioncurrency_account", null);
			}
			[DebuggerNonUserCode]
			set
			{
				this.OnPropertyChanging("TransactioncurrencyAccount");
				this.SetRelatedEntities<Account>("transactioncurrency_account", null, value);
				this.OnPropertyChanged("TransactioncurrencyAccount");
			}
		}

		/// <summary>
		/// 1:N transactioncurrency_contact
		/// </summary>	
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("transactioncurrency_contact")]
		public System.Collections.Generic.IEnumerable<Contact> TransactioncurrencyContact
		{
			[DebuggerNonUserCode]
			get
			{
				return this.GetRelatedEntities<Contact>("transactioncurrency_contact", null);
			}
			[DebuggerNonUserCode]
			set
			{
				this.OnPropertyChanging("TransactioncurrencyContact");
				this.SetRelatedEntities<Contact>("transactioncurrency_contact", null, value);
				this.OnPropertyChanged("TransactioncurrencyContact");
			}
		}

		/// <summary>
		/// 1:N transactioncurrency_usersettings
		/// </summary>	
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("transactioncurrency_usersettings")]
		public System.Collections.Generic.IEnumerable<UserSettings> TransactioncurrencyUsersettings
		{
			[DebuggerNonUserCode]
			get
			{
				return this.GetRelatedEntities<UserSettings>("transactioncurrency_usersettings", null);
			}
			[DebuggerNonUserCode]
			set
			{
				this.OnPropertyChanging("TransactioncurrencyUsersettings");
				this.SetRelatedEntities<UserSettings>("transactioncurrency_usersettings", null, value);
				this.OnPropertyChanged("TransactioncurrencyUsersettings");
			}
		}

		#endregion

		#region Options
		public static class Options
		{
                public struct StateCode
                {
					public const int Active = 0;
					public const int Inactive = 1;
                }
                public struct StatusCode
                {
					public const int Active = 1;
					public const int Inactive = 2;
                }
		}
		#endregion

		#region LogicalNames
		public static class LogicalNames
		{
				public const string TransactionCurrencyId = "transactioncurrencyid";
				public const string CreatedBy = "createdby";
				public const string CreatedOn = "createdon";
				public const string CreatedOnBehalfBy = "createdonbehalfby";
				public const string CurrencyName = "currencyname";
				public const string CurrencyPrecision = "currencyprecision";
				public const string CurrencySymbol = "currencysymbol";
				public const string EntityImage = "entityimage";
				public const string EntityImageTimestamp = "entityimage_timestamp";
				public const string EntityImageURL = "entityimage_url";
				public const string EntityImageId = "entityimageid";
				public const string ExchangeRate = "exchangerate";
				public const string ImportSequenceNumber = "importsequencenumber";
				public const string ISOCurrencyCode = "isocurrencycode";
				public const string ModifiedBy = "modifiedby";
				public const string ModifiedOn = "modifiedon";
				public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
				public const string OrganizationId = "organizationid";
				public const string OverriddenCreatedOn = "overriddencreatedon";
				public const string StateCode = "statecode";
				public const string StatusCode = "statuscode";
				public const string VersionNumber = "versionnumber";
		}
		#endregion

		#region Relations
        public static class Relations
        {
            public static class OneToMany
            {
				public const string AdxInviteredemptionTransactioncurrencyTransactioncurrencyid = "adx_inviteredemption_transactioncurrency_transactioncurrencyid";
				public const string AdxPortalcommentTransactioncurrencyTransactioncurrencyid = "adx_portalcomment_transactioncurrency_transactioncurrencyid";
				public const string BasecurrencyOrganization = "basecurrency_organization";
				public const string ChatTransactioncurrencyTransactioncurrencyid = "chat_transactioncurrency_transactioncurrencyid";
				public const string MsfpAlertTransactioncurrencyTransactioncurrencyid = "msfp_alert_transactioncurrency_transactioncurrencyid";
				public const string MsfpSurveyinviteTransactioncurrencyTransactioncurrencyid = "msfp_surveyinvite_transactioncurrency_transactioncurrencyid";
				public const string MsfpSurveyresponseTransactioncurrencyTransactioncurrencyid = "msfp_surveyresponse_transactioncurrency_transactioncurrencyid";
				public const string TransactioncurrencyAccount = "transactioncurrency_account";
				public const string TransactioncurrencyActioncard = "transactioncurrency_actioncard";
				public const string TransactionCurrencyActionCardUserState = "TransactionCurrency_ActionCardUserState";
				public const string TransactionCurrencyActivityPointer = "TransactionCurrency_ActivityPointer";
				public const string TransactioncurrencyAnnualfiscalcalendar = "transactioncurrency_annualfiscalcalendar";
				public const string TransactionCurrencyAppointment = "TransactionCurrency_Appointment";
				public const string TransactionCurrencyAsyncOperations = "TransactionCurrency_AsyncOperations";
				public const string TransactionCurrencyBusinessUnit = "TransactionCurrency_BusinessUnit";
				public const string TransactioncurrencyCardtype = "transactioncurrency_cardtype";
				public const string TransactioncurrencyCategory = "transactioncurrency_category";
				public const string TransactionCurrencyChannelAccessProfile = "TransactionCurrency_ChannelAccessProfile";
				public const string TransactionCurrencyConnection = "TransactionCurrency_Connection";
				public const string TransactioncurrencyContact = "transactioncurrency_contact";
				public const string TransactionCurrencyConvertRule = "TransactionCurrency_ConvertRule";
				public const string TransactioncurrencyConvertruleitem = "transactioncurrency_convertruleitem";
				public const string TransactionCurrencyCustomerAddress = "TransactionCurrency_CustomerAddress";
				public const string TransactionCurrencyDelveactionhub = "TransactionCurrency_delveactionhub";
				public const string TransactionCurrencyDuplicateBaseRecord = "TransactionCurrency_DuplicateBaseRecord";
				public const string TransactionCurrencyDuplicateMatchingRecord = "TransactionCurrency_DuplicateMatchingRecord";
				public const string TransactionCurrencyEmail = "TransactionCurrency_Email";
				public const string TransactioncurrencyExpiredprocess = "transactioncurrency_expiredprocess";
				public const string TransactionCurrencyExternalParty = "TransactionCurrency_ExternalParty";
				public const string TransactionCurrencyExternalpartyitem = "TransactionCurrency_externalpartyitem";
				public const string TransactionCurrencyFax = "TransactionCurrency_Fax";
				public const string TransactioncurrencyFeedback = "transactioncurrency_feedback";
				public const string TransactioncurrencyFixedmonthlyfiscalcalendar = "transactioncurrency_fixedmonthlyfiscalcalendar";
				public const string TransactionCurrencyGoal = "TransactionCurrency_Goal";
				public const string TransactionCurrencyInteractionForEmail = "TransactionCurrency_InteractionForEmail";
				public const string TransactionCurrencyKbArticle = "TransactionCurrency_KbArticle";
				public const string TransactionCurrencyKnowledgearticle = "TransactionCurrency_knowledgearticle";
				public const string TransactioncurrencyKnowledgearticleviews = "transactioncurrency_knowledgearticleviews";
				public const string TransactionCurrencyKnowledgeBaseRecord = "TransactionCurrency_KnowledgeBaseRecord";
				public const string TransactionCurrencyLetter = "TransactionCurrency_Letter";
				public const string TransactionCurrencyMailMergeTemplate = "TransactionCurrency_MailMergeTemplate";
				public const string TransactioncurrencyMonthlyfiscalcalendar = "transactioncurrency_monthlyfiscalcalendar";
				public const string TransactioncurrencyNewprocess = "transactioncurrency_newprocess";
				public const string TransactionCurrencyOfficegraphdocument = "TransactionCurrency_officegraphdocument";
				public const string TransactionCurrencyPhoneCall = "TransactionCurrency_PhoneCall";
				public const string TransactioncurrencyPosition = "transactioncurrency_position";
				public const string TransactionCurrencyProcessSessions = "TransactionCurrency_ProcessSessions";
				public const string TransactionCurrencyProfilerule = "TransactionCurrency_profilerule";
				public const string TransactionCurrencyProfileruleitem = "TransactionCurrency_profileruleitem";
				public const string TransactioncurrencyQuarterlyfiscalcalendar = "transactioncurrency_quarterlyfiscalcalendar";
				public const string TransactionCurrencyQueue = "TransactionCurrency_Queue";
				public const string TransactionCurrencyQueueItem = "TransactionCurrency_QueueItem";
				public const string TransactionCurrencyRecommendeddocument = "TransactionCurrency_recommendeddocument";
				public const string TransactionCurrencyRecurringAppointmentMaster = "TransactionCurrency_RecurringAppointmentMaster";
				public const string TransactionCurrencyReportCategory = "TransactionCurrency_ReportCategory";
				public const string TransactionCurrencyRoutingrule = "TransactionCurrency_Routingrule";
				public const string TransactionCurrencyRoutingruleitem = "TransactionCurrency_routingruleitem";
				public const string TransactioncurrencySemiannualfiscalcalendar = "transactioncurrency_semiannualfiscalcalendar";
				public const string TransactionCurrencySharePointDocument = "TransactionCurrency_SharePointDocument";
				public const string TransactionCurrencySharePointDocumentLocation = "TransactionCurrency_SharePointDocumentLocation";
				public const string TransactionCurrencySharePointSite = "TransactionCurrency_SharePointSite";
				public const string TransactionCurrencySimilarityRule = "TransactionCurrency_SimilarityRule";
				public const string TransactionCurrencySLA = "TransactionCurrency_SLA";
				public const string TransactionCurrencySLAItem = "TransactionCurrency_SLAItem";
				public const string TransactionCurrencySlakpiinstance = "TransactionCurrency_slakpiinstance";
				public const string TransactioncurrencySocialactivity = "transactioncurrency_socialactivity";
				public const string TransactioncurrencySocialProfile = "transactioncurrency_SocialProfile";
				public const string TransactionCurrencySuggestioncardtemplate = "TransactionCurrency_suggestioncardtemplate";
				public const string TransactionCurrencySyncErrors = "TransactionCurrency_SyncErrors";
				public const string TransactionCurrencySystemUser = "TransactionCurrency_SystemUser";
				public const string TransactionCurrencyTask = "TransactionCurrency_Task";
				public const string TransactionCurrencyTeam = "TransactionCurrency_Team";
				public const string TransactionCurrencyTerritory = "TransactionCurrency_Territory";
				public const string TransactionCurrencyTheme = "TransactionCurrency_Theme";
				public const string TransactioncurrencyTranslationprocess = "transactioncurrency_translationprocess";
				public const string TransactionCurrencyUntrackedEmail = "TransactionCurrency_UntrackedEmail";
				public const string TransactioncurrencyUserfiscalcalendar = "transactioncurrency_userfiscalcalendar";
				public const string TransactionCurrencyUserMapping = "TransactionCurrency_UserMapping";
				public const string TransactioncurrencyUsersettings = "transactioncurrency_usersettings";
				public const string UserentityinstancedataTransactioncurrency = "userentityinstancedata_transactioncurrency";
            }

            public static class ManyToOne
            {
				public const string LkTransactioncurrencyCreatedonbehalfby = "lk_transactioncurrency_createdonbehalfby";
				public const string LkTransactioncurrencyEntityimage = "lk_transactioncurrency_entityimage";
				public const string LkTransactioncurrencyModifiedonbehalfby = "lk_transactioncurrency_modifiedonbehalfby";
				public const string LkTransactioncurrencybaseCreatedby = "lk_transactioncurrencybase_createdby";
				public const string LkTransactioncurrencybaseModifiedby = "lk_transactioncurrencybase_modifiedby";
				public const string OrganizationTransactioncurrencies = "organization_transactioncurrencies";
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
        public static TransactionCurrency Retrieve(IOrganizationService service, Guid id)
        {
            return Retrieve(service,id, new ColumnSet(true));
        }

        public static TransactionCurrency Retrieve(IOrganizationService service, Guid id, ColumnSet columnSet)
        {
            return service.Retrieve("transactioncurrency", id, columnSet).ToEntity<TransactionCurrency>();
        }

        public TransactionCurrency GetChangedEntity()
        {
            if (_trackChanges)
            {
                var attr = new AttributeCollection();
                foreach (var attrName in _changedProperties.Value.Select(changedProperty => ((AttributeLogicalNameAttribute) GetType().GetProperty(changedProperty).GetCustomAttribute(typeof (AttributeLogicalNameAttribute))).LogicalName).Where(attrName => Contains(attrName)))
                {
                    attr.Add(attrName,this[attrName]);
                }
                return new  TransactionCurrency(Id) {Attributes = attr };
            }
            return this;
        }
        #endregion
	}

	#region Context
	public partial class DataContext
	{
		public IQueryable<TransactionCurrency> TransactionCurrencySet
		{
			get
			{
				return CreateQuery<TransactionCurrency>();
			}
		}
	}
	#endregion
}
