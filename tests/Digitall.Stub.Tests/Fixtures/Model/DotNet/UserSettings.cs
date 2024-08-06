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
	/// User's preferred settings.
	/// </summary>
	[DataContractAttribute()]
	[EntityLogicalNameAttribute("usersettings")]
	[System.CodeDom.Compiler.GeneratedCode("dgtp", "2023")]
    [ExcludeFromCodeCoverage]
	public partial class UserSettings : Entity, INotifyPropertyChanging, INotifyPropertyChanged
    {
	    #region ctor
		[DebuggerNonUserCode]
		public UserSettings() : this(false)
        {
        }

        [DebuggerNonUserCode]
		public UserSettings(bool trackChanges = false) : base(EntityLogicalName)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public UserSettings(Guid id, bool trackChanges = false) : base(EntityLogicalName,id)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public UserSettings(KeyAttributeCollection keyAttributes, bool trackChanges = false) : base(EntityLogicalName,keyAttributes)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public UserSettings(string keyName, object keyValue, bool trackChanges = false) : base(EntityLogicalName, keyName, keyValue)
        {
			_trackChanges = trackChanges;
        }
        #endregion

		#region fields
        private readonly bool _trackChanges;
        private readonly Lazy<HashSet<string>> _changedProperties = new Lazy<HashSet<string>>();
        #endregion

        #region consts
        public const string EntityLogicalName = "usersettings";
        public const int EntityTypeCode = 150;
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
		[AttributeLogicalNameAttribute("systemuserid")]
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
				SystemUserId = value;
			}
		}

		/// <summary>
		/// Unique identifier of the user.
		/// </summary>
		[AttributeLogicalName("systemuserid")]
        public Guid? SystemUserId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("systemuserid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(SystemUserId));
                SetAttributeValue("systemuserid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
                OnPropertyChanged(nameof(SystemUserId));
            }
        }

		/// <summary>
		/// Normal polling frequency used for address book synchronization in Microsoft Office Outlook.
		/// </summary>
		[AttributeLogicalName("addressbooksyncinterval")]
        public int? AddressBookSyncInterval
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("addressbooksyncinterval");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(AddressBookSyncInterval));
                SetAttributeValue("addressbooksyncinterval", value);
                OnPropertyChanged(nameof(AddressBookSyncInterval));
            }
        }

		/// <summary>
		/// Default mode, such as simple or detailed, for advanced find.
		/// </summary>
		[AttributeLogicalName("advancedfindstartupmode")]
        public int? AdvancedFindStartupMode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("advancedfindstartupmode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(AdvancedFindStartupMode));
                SetAttributeValue("advancedfindstartupmode", value);
                OnPropertyChanged(nameof(AdvancedFindStartupMode));
            }
        }

		/// <summary>
		/// This attribute is no longer used. The data is now in the Mailbox.AllowEmailConnectorToUseCredentials attribute.
		/// </summary>
		[AttributeLogicalName("allowemailcredentials")]
        public bool? AllowEmailCredentials
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("allowemailcredentials");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(AllowEmailCredentials));
                SetAttributeValue("allowemailcredentials", value);
                OnPropertyChanged(nameof(AllowEmailCredentials));
            }
        }

		/// <summary>
		/// AM designator to use in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("amdesignator")]
        public string AMDesignator
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("amdesignator");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(AMDesignator));
                SetAttributeValue("amdesignator", value);
                OnPropertyChanged(nameof(AMDesignator));
            }
        }

		/// <summary>
		/// Set user status for ADC Suggestions
		/// </summary>
		[AttributeLogicalName("autocaptureuserstatus")]
        public int? AutoCaptureUserStatus
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("autocaptureuserstatus");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(AutoCaptureUserStatus));
                SetAttributeValue("autocaptureuserstatus", value);
                OnPropertyChanged(nameof(AutoCaptureUserStatus));
            }
        }

		/// <summary>
		/// Auto-create contact on client promote
		/// </summary>
		[AttributeLogicalName("autocreatecontactonpromote")]
        public int? AutoCreateContactOnPromote
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("autocreatecontactonpromote");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(AutoCreateContactOnPromote));
                SetAttributeValue("autocreatecontactonpromote", value);
                OnPropertyChanged(nameof(AutoCreateContactOnPromote));
            }
        }

		/// <summary>
		/// Unique identifier of the business unit with which the user is associated.
		/// </summary>
		[AttributeLogicalName("businessunitid")]
        public Guid? BusinessUnitId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("businessunitid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(BusinessUnitId));
                SetAttributeValue("businessunitid", value);
                OnPropertyChanged(nameof(BusinessUnitId));
            }
        }

		/// <summary>
		/// Calendar type for the system. Set to Gregorian US by default.
		/// </summary>
		[AttributeLogicalName("calendartype")]
        public int? CalendarType
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("calendartype");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(CalendarType));
                SetAttributeValue("calendartype", value);
                OnPropertyChanged(nameof(CalendarType));
            }
        }

		/// <summary>
		/// Unique identifier of the user who created the user settings.
		/// </summary>
		[AttributeLogicalName("createdby")]
        public EntityReference CreatedBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("createdby");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(CreatedBy));
                SetAttributeValue("createdby", value);
                OnPropertyChanged(nameof(CreatedBy));
            }
        }

		/// <summary>
		/// Date and time when the user settings object was created.
		/// </summary>
		[AttributeLogicalName("createdon")]
        public DateTime? CreatedOn
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<DateTime?>("createdon");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(CreatedOn));
                SetAttributeValue("createdon", value);
                OnPropertyChanged(nameof(CreatedOn));
            }
        }

		/// <summary>
		/// Unique identifier of the delegate user who created the usersettings.
		/// </summary>
		[AttributeLogicalName("createdonbehalfby")]
        public EntityReference CreatedOnBehalfBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("createdonbehalfby");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(CreatedOnBehalfBy));
                SetAttributeValue("createdonbehalfby", value);
                OnPropertyChanged(nameof(CreatedOnBehalfBy));
            }
        }

		/// <summary>
		/// Number of decimal places that can be used for currency.
		/// </summary>
		[AttributeLogicalName("currencydecimalprecision")]
        public int? CurrencyDecimalPrecision
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("currencydecimalprecision");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(CurrencyDecimalPrecision));
                SetAttributeValue("currencydecimalprecision", value);
                OnPropertyChanged(nameof(CurrencyDecimalPrecision));
            }
        }

		/// <summary>
		/// Information about how currency symbols are placed in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("currencyformatcode")]
        public int? CurrencyFormatCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("currencyformatcode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(CurrencyFormatCode));
                SetAttributeValue("currencyformatcode", value);
                OnPropertyChanged(nameof(CurrencyFormatCode));
            }
        }

		/// <summary>
		/// Symbol used for currency in Microsoft Dynamics 365.
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
		/// Determines the status of auto install of Dynamics 365 to Teams attempt has been completed
		/// </summary>
		[AttributeLogicalName("d365autoinstallattemptstatus")]
        public OptionSetValue D365AutoInstallAttemptStatus
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("d365autoinstallattemptstatus");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(D365AutoInstallAttemptStatus));
                SetAttributeValue("d365autoinstallattemptstatus", value);
                OnPropertyChanged(nameof(D365AutoInstallAttemptStatus));
            }
        }

		/// <summary>
		/// Information that specifies the level of data validation in excel worksheets exported in a format suitable for import.
		/// </summary>
		[AttributeLogicalName("datavalidationmodeforexporttoexcel")]
        public OptionSetValue DataValidationModeForExportToExcel
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("datavalidationmodeforexporttoexcel");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DataValidationModeForExportToExcel));
                SetAttributeValue("datavalidationmodeforexporttoexcel", value);
                OnPropertyChanged(nameof(DataValidationModeForExportToExcel));
            }
        }

		/// <summary>
		/// Information about how the date is displayed in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("dateformatcode")]
        public int? DateFormatCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("dateformatcode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DateFormatCode));
                SetAttributeValue("dateformatcode", value);
                OnPropertyChanged(nameof(DateFormatCode));
            }
        }

		/// <summary>
		/// String showing how the date is displayed throughout Microsoft 365.
		/// </summary>
		[AttributeLogicalName("dateformatstring")]
        public string DateFormatString
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("dateformatstring");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DateFormatString));
                SetAttributeValue("dateformatstring", value);
                OnPropertyChanged(nameof(DateFormatString));
            }
        }

		/// <summary>
		/// Character used to separate the month, the day, and the year in dates in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("dateseparator")]
        public string DateSeparator
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("dateseparator");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DateSeparator));
                SetAttributeValue("dateseparator", value);
                OnPropertyChanged(nameof(DateSeparator));
            }
        }

		/// <summary>
		/// Symbol used for decimal in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("decimalsymbol")]
        public string DecimalSymbol
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("decimalsymbol");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DecimalSymbol));
                SetAttributeValue("decimalsymbol", value);
                OnPropertyChanged(nameof(DecimalSymbol));
            }
        }

		/// <summary>
		/// Default calendar view for the user.
		/// </summary>
		[AttributeLogicalName("defaultcalendarview")]
        public int? DefaultCalendarView
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("defaultcalendarview");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DefaultCalendarView));
                SetAttributeValue("defaultcalendarview", value);
                OnPropertyChanged(nameof(DefaultCalendarView));
            }
        }

		/// <summary>
		/// Text area to enter default country code.
		/// </summary>
		[AttributeLogicalName("defaultcountrycode")]
        public string DefaultCountryCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("defaultcountrycode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DefaultCountryCode));
                SetAttributeValue("defaultcountrycode", value);
                OnPropertyChanged(nameof(DefaultCountryCode));
            }
        }

		/// <summary>
		/// Unique identifier of the default dashboard.
		/// </summary>
		[AttributeLogicalName("defaultdashboardid")]
        public Guid? DefaultDashboardId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("defaultdashboardid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DefaultDashboardId));
                SetAttributeValue("defaultdashboardid", value);
                OnPropertyChanged(nameof(DefaultDashboardId));
            }
        }

		/// <summary>
		/// Default search experience for the user.
		/// </summary>
		[AttributeLogicalName("defaultsearchexperience")]
        public OptionSetValue DefaultSearchExperience
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("defaultsearchexperience");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DefaultSearchExperience));
                SetAttributeValue("defaultsearchexperience", value);
                OnPropertyChanged(nameof(DefaultSearchExperience));
            }
        }

		/// <summary>
		/// This attribute is no longer used. The data is now in the Mailbox.Password attribute.
		/// </summary>
		[AttributeLogicalName("emailpassword")]
        public string EmailPassword
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("emailpassword");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(EmailPassword));
                SetAttributeValue("emailpassword", value);
                OnPropertyChanged(nameof(EmailPassword));
            }
        }

		/// <summary>
		/// This attribute is no longer used. The data is now in the Mailbox.UserName attribute.
		/// </summary>
		[AttributeLogicalName("emailusername")]
        public string EmailUsername
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("emailusername");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(EmailUsername));
                SetAttributeValue("emailusername", value);
                OnPropertyChanged(nameof(EmailUsername));
            }
        }

		/// <summary>
		/// Indicates the form mode to be used.
		/// </summary>
		[AttributeLogicalName("entityformmode")]
        public OptionSetValue EntityFormMode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("entityformmode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(EntityFormMode));
                SetAttributeValue("entityformmode", value);
                OnPropertyChanged(nameof(EntityFormMode));
            }
        }

		/// <summary>
		/// Order in which names are to be displayed in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("fullnameconventioncode")]
        public int? FullNameConventionCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("fullnameconventioncode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(FullNameConventionCode));
                SetAttributeValue("fullnameconventioncode", value);
                OnPropertyChanged(nameof(FullNameConventionCode));
            }
        }

		/// <summary>
		/// Information that specifies whether the Get Started pane in lists is enabled.
		/// </summary>
		[AttributeLogicalName("getstartedpanecontentenabled")]
        public bool? GetStartedPaneContentEnabled
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("getstartedpanecontentenabled");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(GetStartedPaneContentEnabled));
                SetAttributeValue("getstartedpanecontentenabled", value);
                OnPropertyChanged(nameof(GetStartedPaneContentEnabled));
            }
        }

		/// <summary>
		/// Unique identifier of the Help language.
		/// </summary>
		[AttributeLogicalName("helplanguageid")]
        public int? HelpLanguageId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("helplanguageid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(HelpLanguageId));
                SetAttributeValue("helplanguageid", value);
                OnPropertyChanged(nameof(HelpLanguageId));
            }
        }

		/// <summary>
		/// Web site home page for the user.
		/// </summary>
		[AttributeLogicalName("homepagearea")]
        public string HomepageArea
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("homepagearea");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(HomepageArea));
                SetAttributeValue("homepagearea", value);
                OnPropertyChanged(nameof(HomepageArea));
            }
        }

		/// <summary>
		/// Configuration of the home page layout.
		/// </summary>
		[AttributeLogicalName("homepagelayout")]
        public string HomepageLayout
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("homepagelayout");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(HomepageLayout));
                SetAttributeValue("homepagelayout", value);
                OnPropertyChanged(nameof(HomepageLayout));
            }
        }

		/// <summary>
		/// Web site page for the user.
		/// </summary>
		[AttributeLogicalName("homepagesubarea")]
        public string HomepageSubarea
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("homepagesubarea");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(HomepageSubarea));
                SetAttributeValue("homepagesubarea", value);
                OnPropertyChanged(nameof(HomepageSubarea));
            }
        }

		/// <summary>
		/// Information that specifies whether a user account is to ignore unsolicited email (deprecated).
		/// </summary>
		[AttributeLogicalName("ignoreunsolicitedemail")]
        public bool? IgnoreUnsolicitedEmail
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("ignoreunsolicitedemail");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IgnoreUnsolicitedEmail));
                SetAttributeValue("ignoreunsolicitedemail", value);
                OnPropertyChanged(nameof(IgnoreUnsolicitedEmail));
            }
        }

		/// <summary>
		/// Incoming email filtering method.
		/// </summary>
		[AttributeLogicalName("incomingemailfilteringmethod")]
        public OptionSetValue IncomingEmailFilteringMethod
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("incomingemailfilteringmethod");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IncomingEmailFilteringMethod));
                SetAttributeValue("incomingemailfilteringmethod", value);
                OnPropertyChanged(nameof(IncomingEmailFilteringMethod));
            }
        }

		/// <summary>
		/// Show or dismiss alert for Apps for 365.
		/// </summary>
		[AttributeLogicalName("isappsforcrmalertdismissed")]
        public bool? IsAppsForCrmAlertDismissed
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("isappsforcrmalertdismissed");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IsAppsForCrmAlertDismissed));
                SetAttributeValue("isappsforcrmalertdismissed", value);
                OnPropertyChanged(nameof(IsAppsForCrmAlertDismissed));
            }
        }

		/// <summary>
		/// Indicates whether to use the Auto Capture feature enabled or not.
		/// </summary>
		[AttributeLogicalName("isautodatacaptureenabled")]
        public bool? IsAutoDataCaptureEnabled
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("isautodatacaptureenabled");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IsAutoDataCaptureEnabled));
                SetAttributeValue("isautodatacaptureenabled", value);
                OnPropertyChanged(nameof(IsAutoDataCaptureEnabled));
            }
        }

		/// <summary>
		/// Enable or disable country code selection .
		/// </summary>
		[AttributeLogicalName("isdefaultcountrycodecheckenabled")]
        public bool? IsDefaultCountryCodeCheckEnabled
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("isdefaultcountrycodecheckenabled");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IsDefaultCountryCodeCheckEnabled));
                SetAttributeValue("isdefaultcountrycodecheckenabled", value);
                OnPropertyChanged(nameof(IsDefaultCountryCodeCheckEnabled));
            }
        }

		/// <summary>
		/// Indicates if duplicate detection is enabled when going online.
		/// </summary>
		[AttributeLogicalName("isduplicatedetectionenabledwhengoingonline")]
        public bool? IsDuplicateDetectionEnabledWhenGoingOnline
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("isduplicatedetectionenabledwhengoingonline");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IsDuplicateDetectionEnabledWhenGoingOnline));
                SetAttributeValue("isduplicatedetectionenabledwhengoingonline", value);
                OnPropertyChanged(nameof(IsDuplicateDetectionEnabledWhenGoingOnline));
            }
        }

		/// <summary>
		/// Enable or disable email conversation view on timeline wall selection.
		/// </summary>
		[AttributeLogicalName("isemailconversationviewenabled")]
        public bool? IsEmailConversationViewEnabled
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("isemailconversationviewenabled");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IsEmailConversationViewEnabled));
                SetAttributeValue("isemailconversationviewenabled", value);
                OnPropertyChanged(nameof(IsEmailConversationViewEnabled));
            }
        }

		/// <summary>
		/// Enable or disable guided help.
		/// </summary>
		[AttributeLogicalName("isguidedhelpenabled")]
        public bool? IsGuidedHelpEnabled
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("isguidedhelpenabled");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IsGuidedHelpEnabled));
                SetAttributeValue("isguidedhelpenabled", value);
                OnPropertyChanged(nameof(IsGuidedHelpEnabled));
            }
        }

		/// <summary>
		/// Indicates if the synchronization of user resource booking with Exchange is enabled at user level.
		/// </summary>
		[AttributeLogicalName("isresourcebookingexchangesyncenabled")]
        public bool? IsResourceBookingExchangeSyncEnabled
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("isresourcebookingexchangesyncenabled");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IsResourceBookingExchangeSyncEnabled));
                SetAttributeValue("isresourcebookingexchangesyncenabled", value);
                OnPropertyChanged(nameof(IsResourceBookingExchangeSyncEnabled));
            }
        }

		/// <summary>
		/// Indicates if send as other user privilege is enabled or not.
		/// </summary>
		[AttributeLogicalName("issendasallowed")]
        public bool? IsSendAsAllowed
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("issendasallowed");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IsSendAsAllowed));
                SetAttributeValue("issendasallowed", value);
                OnPropertyChanged(nameof(IsSendAsAllowed));
            }
        }

		/// <summary>
		/// Shows the last time when the traces were read from the database.
		/// </summary>
		[AttributeLogicalName("lastalertsviewedtime")]
        public DateTime? LastAlertsViewedTime
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<DateTime?>("lastalertsviewedtime");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(LastAlertsViewedTime));
                SetAttributeValue("lastalertsviewedtime", value);
                OnPropertyChanged(nameof(LastAlertsViewedTime));
            }
        }

		/// <summary>
		/// Stores the timestamp for when the ViewPersonalizationSettings attribute was updated for this user in the UserEntityUISettings table.
		/// </summary>
		[AttributeLogicalName("lastmodifiedtimeforviewpersonalizationsettings")]
        public DateTime? LastModifiedTimeForViewPersonalizationSettings
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<DateTime?>("lastmodifiedtimeforviewpersonalizationsettings");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(LastModifiedTimeForViewPersonalizationSettings));
                SetAttributeValue("lastmodifiedtimeforviewpersonalizationsettings", value);
                OnPropertyChanged(nameof(LastModifiedTimeForViewPersonalizationSettings));
            }
        }

		/// <summary>
		/// Unique identifier of the user locale.
		/// </summary>
		[AttributeLogicalName("localeid")]
        public int? LocaleId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("localeid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(LocaleId));
                SetAttributeValue("localeid", value);
                OnPropertyChanged(nameof(LocaleId));
            }
        }

		/// <summary>
		/// Information that specifies how Long Date is displayed throughout Microsoft 365.
		/// </summary>
		[AttributeLogicalName("longdateformatcode")]
        public int? LongDateFormatCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("longdateformatcode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(LongDateFormatCode));
                SetAttributeValue("longdateformatcode", value);
                OnPropertyChanged(nameof(LongDateFormatCode));
            }
        }

		/// <summary>
		/// Unique identifier of the user who last modified the user settings.
		/// </summary>
		[AttributeLogicalName("modifiedby")]
        public EntityReference ModifiedBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("modifiedby");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ModifiedBy));
                SetAttributeValue("modifiedby", value);
                OnPropertyChanged(nameof(ModifiedBy));
            }
        }

		/// <summary>
		/// Date and time when the user settings object was last modified.
		/// </summary>
		[AttributeLogicalName("modifiedon")]
        public DateTime? ModifiedOn
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<DateTime?>("modifiedon");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ModifiedOn));
                SetAttributeValue("modifiedon", value);
                OnPropertyChanged(nameof(ModifiedOn));
            }
        }

		/// <summary>
		/// Unique identifier of the delegate user who last modified the usersettings.
		/// </summary>
		[AttributeLogicalName("modifiedonbehalfby")]
        public EntityReference ModifiedOnBehalfBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("modifiedonbehalfby");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ModifiedOnBehalfBy));
                SetAttributeValue("modifiedonbehalfby", value);
                OnPropertyChanged(nameof(ModifiedOnBehalfBy));
            }
        }

		/// <summary>
		/// Information that specifies how negative currency numbers are displayed in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("negativecurrencyformatcode")]
        public int? NegativeCurrencyFormatCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("negativecurrencyformatcode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(NegativeCurrencyFormatCode));
                SetAttributeValue("negativecurrencyformatcode", value);
                OnPropertyChanged(nameof(NegativeCurrencyFormatCode));
            }
        }

		/// <summary>
		/// Information that specifies how negative numbers are displayed in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("negativeformatcode")]
        public int? NegativeFormatCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("negativeformatcode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(NegativeFormatCode));
                SetAttributeValue("negativeformatcode", value);
                OnPropertyChanged(nameof(NegativeFormatCode));
            }
        }

		/// <summary>
		/// Next tracking number.
		/// </summary>
		[AttributeLogicalName("nexttrackingnumber")]
        public int? NextTrackingNumber
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("nexttrackingnumber");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(NextTrackingNumber));
                SetAttributeValue("nexttrackingnumber", value);
                OnPropertyChanged(nameof(NextTrackingNumber));
            }
        }

		/// <summary>
		/// Information that specifies how numbers are grouped in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("numbergroupformat")]
        public string NumberGroupFormat
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("numbergroupformat");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(NumberGroupFormat));
                SetAttributeValue("numbergroupformat", value);
                OnPropertyChanged(nameof(NumberGroupFormat));
            }
        }

		/// <summary>
		/// Symbol used for number separation in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("numberseparator")]
        public string NumberSeparator
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("numberseparator");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(NumberSeparator));
                SetAttributeValue("numberseparator", value);
                OnPropertyChanged(nameof(NumberSeparator));
            }
        }

		/// <summary>
		/// Normal polling frequency used for background offline synchronization in Microsoft Office Outlook.
		/// </summary>
		[AttributeLogicalName("offlinesyncinterval")]
        public int? OfflineSyncInterval
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("offlinesyncinterval");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(OfflineSyncInterval));
                SetAttributeValue("offlinesyncinterval", value);
                OnPropertyChanged(nameof(OfflineSyncInterval));
            }
        }

		/// <summary>
		/// Normal polling frequency used for record synchronization in Microsoft Office Outlook.
		/// </summary>
		[AttributeLogicalName("outlooksyncinterval")]
        public int? OutlookSyncInterval
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("outlooksyncinterval");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(OutlookSyncInterval));
                SetAttributeValue("outlooksyncinterval", value);
                OnPropertyChanged(nameof(OutlookSyncInterval));
            }
        }

		/// <summary>
		/// Information that specifies how many items to list on a page in list views.
		/// </summary>
		[AttributeLogicalName("paginglimit")]
        public int? PagingLimit
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("paginglimit");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(PagingLimit));
                SetAttributeValue("paginglimit", value);
                OnPropertyChanged(nameof(PagingLimit));
            }
        }

		/// <summary>
		/// For internal use only.
		/// </summary>
		[AttributeLogicalName("personalizationsettings")]
        public string PersonalizationSettings
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("personalizationsettings");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(PersonalizationSettings));
                SetAttributeValue("personalizationsettings", value);
                OnPropertyChanged(nameof(PersonalizationSettings));
            }
        }

		/// <summary>
		/// PM designator to use in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("pmdesignator")]
        public string PMDesignator
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("pmdesignator");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(PMDesignator));
                SetAttributeValue("pmdesignator", value);
                OnPropertyChanged(nameof(PMDesignator));
            }
        }

		/// <summary>
		/// Preferred Solution when create a component without under a solution in this organization
		/// </summary>
		[AttributeLogicalName("preferredsolution")]
        public EntityReference PreferredSolution
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("preferredsolution");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(PreferredSolution));
                SetAttributeValue("preferredsolution", value);
                OnPropertyChanged(nameof(PreferredSolution));
            }
        }

		/// <summary>
		/// Number of decimal places that can be used for prices.
		/// </summary>
		[AttributeLogicalName("pricingdecimalprecision")]
        public int? PricingDecimalPrecision
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("pricingdecimalprecision");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(PricingDecimalPrecision));
                SetAttributeValue("pricingdecimalprecision", value);
                OnPropertyChanged(nameof(PricingDecimalPrecision));
            }
        }

		/// <summary>
		/// Model app channel override
		/// </summary>
		[AttributeLogicalName("releasechannel")]
        public OptionSetValue ReleaseChannel
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("releasechannel");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ReleaseChannel));
                SetAttributeValue("releasechannel", value);
                OnPropertyChanged(nameof(ReleaseChannel));
            }
        }

		/// <summary>
		/// Picklist for selecting the user preference for reporting scripting errors.
		/// </summary>
		[AttributeLogicalName("reportscripterrors")]
        public OptionSetValue ReportScriptErrors
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("reportscripterrors");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ReportScriptErrors));
                SetAttributeValue("reportscripterrors", value);
                OnPropertyChanged(nameof(ReportScriptErrors));
            }
        }

		/// <summary>
		/// The version number for resource booking synchronization with Exchange.
		/// </summary>
		[AttributeLogicalName("resourcebookingexchangesyncversion")]
        public long? ResourceBookingExchangeSyncVersion
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<long?>("resourcebookingexchangesyncversion");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ResourceBookingExchangeSyncVersion));
                SetAttributeValue("resourcebookingexchangesyncversion", value);
                OnPropertyChanged(nameof(ResourceBookingExchangeSyncVersion));
            }
        }

		/// <summary>
		/// Store selected customer service hub dashboard saved filter id.
		/// </summary>
		[AttributeLogicalName("selectedglobalfilterid")]
        public Guid? SelectedGlobalFilterId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("selectedglobalfilterid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(SelectedGlobalFilterId));
                SetAttributeValue("selectedglobalfilterid", value);
                OnPropertyChanged(nameof(SelectedGlobalFilterId));
            }
        }

		/// <summary>
		/// Information that specifies whether to display the week number in calendar displays in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("showweeknumber")]
        public bool? ShowWeekNumber
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("showweeknumber");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ShowWeekNumber));
                SetAttributeValue("showweeknumber", value);
                OnPropertyChanged(nameof(ShowWeekNumber));
            }
        }

		/// <summary>
		/// For Internal use only
		/// </summary>
		[AttributeLogicalName("splitviewstate")]
        public bool? SplitViewState
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("splitviewstate");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(SplitViewState));
                SetAttributeValue("splitviewstate", value);
                OnPropertyChanged(nameof(SplitViewState));
            }
        }

		/// <summary>
		/// Indicates if the company field in Microsoft Office Outlook items are set during Outlook synchronization.
		/// </summary>
		[AttributeLogicalName("synccontactcompany")]
        public bool? SyncContactCompany
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("synccontactcompany");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(SyncContactCompany));
                SetAttributeValue("synccontactcompany", value);
                OnPropertyChanged(nameof(SyncContactCompany));
            }
        }

		/// <summary>
		/// The number of times a user has interacted with the Tabled Scoped Dataverse Search feature teaching bubble.
		/// </summary>
		[AttributeLogicalName("tablescopeddvsearchfeatureteachingbubbleviews")]
        public int? TableScopedDVSearchFeatureTeachingBubbleViews
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("tablescopeddvsearchfeatureteachingbubbleviews");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TableScopedDVSearchFeatureTeachingBubbleViews));
                SetAttributeValue("tablescopeddvsearchfeatureteachingbubbleviews", value);
                OnPropertyChanged(nameof(TableScopedDVSearchFeatureTeachingBubbleViews));
            }
        }

		/// <summary>
		/// The number of times a user has interacted with the Tabled Scoped Dataverse Search Quick Find teaching bubble.
		/// </summary>
		[AttributeLogicalName("tablescopeddvsearchquickfindteachingbubbleviews")]
        public int? TableScopedDVSearchQuickFindTeachingBubbleViews
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("tablescopeddvsearchquickfindteachingbubbleviews");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TableScopedDVSearchQuickFindTeachingBubbleViews));
                SetAttributeValue("tablescopeddvsearchquickfindteachingbubbleviews", value);
                OnPropertyChanged(nameof(TableScopedDVSearchQuickFindTeachingBubbleViews));
            }
        }

		/// <summary>
		/// Information that specifies how the time is displayed in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("timeformatcode")]
        public int? TimeFormatCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timeformatcode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeFormatCode));
                SetAttributeValue("timeformatcode", value);
                OnPropertyChanged(nameof(TimeFormatCode));
            }
        }

		/// <summary>
		/// Text for how time is displayed in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("timeformatstring")]
        public string TimeFormatString
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("timeformatstring");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeFormatString));
                SetAttributeValue("timeformatstring", value);
                OnPropertyChanged(nameof(TimeFormatString));
            }
        }

		/// <summary>
		/// Text for how time is displayed in Microsoft Dynamics 365.
		/// </summary>
		[AttributeLogicalName("timeseparator")]
        public string TimeSeparator
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("timeseparator");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeSeparator));
                SetAttributeValue("timeseparator", value);
                OnPropertyChanged(nameof(TimeSeparator));
            }
        }

		/// <summary>
		/// Local time zone adjustment for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonebias")]
        public int? TimeZoneBias
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonebias");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneBias));
                SetAttributeValue("timezonebias", value);
                OnPropertyChanged(nameof(TimeZoneBias));
            }
        }

		/// <summary>
		/// Local time zone for the user.
		/// </summary>
		[AttributeLogicalName("timezonecode")]
        public int? TimeZoneCode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonecode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneCode));
                SetAttributeValue("timezonecode", value);
                OnPropertyChanged(nameof(TimeZoneCode));
            }
        }

		/// <summary>
		/// Local time zone daylight adjustment for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonedaylightbias")]
        public int? TimeZoneDaylightBias
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonedaylightbias");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneDaylightBias));
                SetAttributeValue("timezonedaylightbias", value);
                OnPropertyChanged(nameof(TimeZoneDaylightBias));
            }
        }

		/// <summary>
		/// Local time zone daylight day for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonedaylightday")]
        public int? TimeZoneDaylightDay
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonedaylightday");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneDaylightDay));
                SetAttributeValue("timezonedaylightday", value);
                OnPropertyChanged(nameof(TimeZoneDaylightDay));
            }
        }

		/// <summary>
		/// Local time zone daylight day of week for the user. System calculated based on the time zone selected in Options.
		/// </summary>
		[AttributeLogicalName("timezonedaylightdayofweek")]
        public int? TimeZoneDaylightDayOfWeek
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonedaylightdayofweek");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneDaylightDayOfWeek));
                SetAttributeValue("timezonedaylightdayofweek", value);
                OnPropertyChanged(nameof(TimeZoneDaylightDayOfWeek));
            }
        }

		/// <summary>
		/// Local time zone daylight hour for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonedaylighthour")]
        public int? TimeZoneDaylightHour
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonedaylighthour");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneDaylightHour));
                SetAttributeValue("timezonedaylighthour", value);
                OnPropertyChanged(nameof(TimeZoneDaylightHour));
            }
        }

		/// <summary>
		/// Local time zone daylight minute for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonedaylightminute")]
        public int? TimeZoneDaylightMinute
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonedaylightminute");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneDaylightMinute));
                SetAttributeValue("timezonedaylightminute", value);
                OnPropertyChanged(nameof(TimeZoneDaylightMinute));
            }
        }

		/// <summary>
		/// Local time zone daylight month for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonedaylightmonth")]
        public int? TimeZoneDaylightMonth
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonedaylightmonth");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneDaylightMonth));
                SetAttributeValue("timezonedaylightmonth", value);
                OnPropertyChanged(nameof(TimeZoneDaylightMonth));
            }
        }

		/// <summary>
		/// Local time zone daylight second for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonedaylightsecond")]
        public int? TimeZoneDaylightSecond
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonedaylightsecond");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneDaylightSecond));
                SetAttributeValue("timezonedaylightsecond", value);
                OnPropertyChanged(nameof(TimeZoneDaylightSecond));
            }
        }

		/// <summary>
		/// Local time zone daylight year for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonedaylightyear")]
        public int? TimeZoneDaylightYear
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonedaylightyear");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneDaylightYear));
                SetAttributeValue("timezonedaylightyear", value);
                OnPropertyChanged(nameof(TimeZoneDaylightYear));
            }
        }

		/// <summary>
		/// Local time zone standard time bias for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonestandardbias")]
        public int? TimeZoneStandardBias
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonestandardbias");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneStandardBias));
                SetAttributeValue("timezonestandardbias", value);
                OnPropertyChanged(nameof(TimeZoneStandardBias));
            }
        }

		/// <summary>
		/// Local time zone standard day for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonestandardday")]
        public int? TimeZoneStandardDay
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonestandardday");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneStandardDay));
                SetAttributeValue("timezonestandardday", value);
                OnPropertyChanged(nameof(TimeZoneStandardDay));
            }
        }

		/// <summary>
		/// Local time zone standard day of week for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonestandarddayofweek")]
        public int? TimeZoneStandardDayOfWeek
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonestandarddayofweek");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneStandardDayOfWeek));
                SetAttributeValue("timezonestandarddayofweek", value);
                OnPropertyChanged(nameof(TimeZoneStandardDayOfWeek));
            }
        }

		/// <summary>
		/// Local time zone standard hour for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonestandardhour")]
        public int? TimeZoneStandardHour
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonestandardhour");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneStandardHour));
                SetAttributeValue("timezonestandardhour", value);
                OnPropertyChanged(nameof(TimeZoneStandardHour));
            }
        }

		/// <summary>
		/// Local time zone standard minute for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonestandardminute")]
        public int? TimeZoneStandardMinute
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonestandardminute");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneStandardMinute));
                SetAttributeValue("timezonestandardminute", value);
                OnPropertyChanged(nameof(TimeZoneStandardMinute));
            }
        }

		/// <summary>
		/// Local time zone standard month for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonestandardmonth")]
        public int? TimeZoneStandardMonth
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonestandardmonth");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneStandardMonth));
                SetAttributeValue("timezonestandardmonth", value);
                OnPropertyChanged(nameof(TimeZoneStandardMonth));
            }
        }

		/// <summary>
		/// Local time zone standard second for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonestandardsecond")]
        public int? TimeZoneStandardSecond
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonestandardsecond");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneStandardSecond));
                SetAttributeValue("timezonestandardsecond", value);
                OnPropertyChanged(nameof(TimeZoneStandardSecond));
            }
        }

		/// <summary>
		/// Local time zone standard year for the user. System calculated based on the time zone selected.
		/// </summary>
		[AttributeLogicalName("timezonestandardyear")]
        public int? TimeZoneStandardYear
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("timezonestandardyear");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TimeZoneStandardYear));
                SetAttributeValue("timezonestandardyear", value);
                OnPropertyChanged(nameof(TimeZoneStandardYear));
            }
        }

		/// <summary>
		/// Tracking token ID.
		/// </summary>
		[AttributeLogicalName("trackingtokenid")]
        public int? TrackingTokenId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("trackingtokenid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TrackingTokenId));
                SetAttributeValue("trackingtokenid", value);
                OnPropertyChanged(nameof(TrackingTokenId));
            }
        }

		/// <summary>
		/// Unique identifier of the default currency of the user.
		/// </summary>
		[AttributeLogicalName("transactioncurrencyid")]
        public EntityReference TransactionCurrencyId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("transactioncurrencyid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TransactionCurrencyId));
                SetAttributeValue("transactioncurrencyid", value);
                OnPropertyChanged(nameof(TransactionCurrencyId));
            }
        }

		/// <summary>
		/// The list of app modules with try toggle sets
		/// </summary>
		[AttributeLogicalName("trytogglesets")]
        public string TryToggleSets
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("trytogglesets");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TryToggleSets));
                SetAttributeValue("trytogglesets", value);
                OnPropertyChanged(nameof(TryToggleSets));
            }
        }

		/// <summary>
		/// Enable or disable try toggle status.
		/// </summary>
		[AttributeLogicalName("trytogglestatus")]
        public bool? TryToggleStatus
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("trytogglestatus");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TryToggleStatus));
                SetAttributeValue("trytogglestatus", value);
                OnPropertyChanged(nameof(TryToggleStatus));
            }
        }

		/// <summary>
		/// Unique identifier of the language in which to view the user interface (UI).
		/// </summary>
		[AttributeLogicalName("uilanguageid")]
        public int? UILanguageId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("uilanguageid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(UILanguageId));
                SetAttributeValue("uilanguageid", value);
                OnPropertyChanged(nameof(UILanguageId));
            }
        }

		/// <summary>
		/// Indicates whether to use the Microsoft Dynamics 365 appointment form within Microsoft Office Outlook for creating new appointments.
		/// </summary>
		[AttributeLogicalName("usecrmformforappointment")]
        public bool? UseCrmFormForAppointment
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("usecrmformforappointment");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(UseCrmFormForAppointment));
                SetAttributeValue("usecrmformforappointment", value);
                OnPropertyChanged(nameof(UseCrmFormForAppointment));
            }
        }

		/// <summary>
		/// Indicates whether to use the Microsoft Dynamics 365 contact form within Microsoft Office Outlook for creating new contacts.
		/// </summary>
		[AttributeLogicalName("usecrmformforcontact")]
        public bool? UseCrmFormForContact
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("usecrmformforcontact");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(UseCrmFormForContact));
                SetAttributeValue("usecrmformforcontact", value);
                OnPropertyChanged(nameof(UseCrmFormForContact));
            }
        }

		/// <summary>
		/// Indicates whether to use the Microsoft Dynamics 365 email form within Microsoft Office Outlook for creating new emails.
		/// </summary>
		[AttributeLogicalName("usecrmformforemail")]
        public bool? UseCrmFormForEmail
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("usecrmformforemail");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(UseCrmFormForEmail));
                SetAttributeValue("usecrmformforemail", value);
                OnPropertyChanged(nameof(UseCrmFormForEmail));
            }
        }

		/// <summary>
		/// Indicates whether to use the Microsoft Dynamics 365 task form within Microsoft Office Outlook for creating new tasks.
		/// </summary>
		[AttributeLogicalName("usecrmformfortask")]
        public bool? UseCrmFormForTask
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("usecrmformfortask");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(UseCrmFormForTask));
                SetAttributeValue("usecrmformfortask", value);
                OnPropertyChanged(nameof(UseCrmFormForTask));
            }
        }

		/// <summary>
		/// Indicates whether image strips are used to render images.
		/// </summary>
		[AttributeLogicalName("useimagestrips")]
        public bool? UseImageStrips
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("useimagestrips");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(UseImageStrips));
                SetAttributeValue("useimagestrips", value);
                OnPropertyChanged(nameof(UseImageStrips));
            }
        }

		/// <summary>
		/// Specifies user profile ids in comma separated list.
		/// </summary>
		[AttributeLogicalName("userprofile")]
        public string UserProfile
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("userprofile");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(UserProfile));
                SetAttributeValue("userprofile", value);
                OnPropertyChanged(nameof(UserProfile));
            }
        }

		
		[AttributeLogicalName("versionnumber")]
        public long? VersionNumber
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<long?>("versionnumber");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(VersionNumber));
                SetAttributeValue("versionnumber", value);
                OnPropertyChanged(nameof(VersionNumber));
            }
        }

		/// <summary>
		/// The layout of the visualization pane.
		/// </summary>
		[AttributeLogicalName("visualizationpanelayout")]
        public OptionSetValue VisualizationPaneLayout
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("visualizationpanelayout");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(VisualizationPaneLayout));
                SetAttributeValue("visualizationpanelayout", value);
                OnPropertyChanged(nameof(VisualizationPaneLayout));
            }
        }

		/// <summary>
		/// Workday start time for the user.
		/// </summary>
		[AttributeLogicalName("workdaystarttime")]
        public string WorkdayStartTime
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("workdaystarttime");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(WorkdayStartTime));
                SetAttributeValue("workdaystarttime", value);
                OnPropertyChanged(nameof(WorkdayStartTime));
            }
        }

		/// <summary>
		/// Workday stop time for the user.
		/// </summary>
		[AttributeLogicalName("workdaystoptime")]
        public string WorkdayStopTime
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("workdaystoptime");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(WorkdayStopTime));
                SetAttributeValue("workdaystoptime", value);
                OnPropertyChanged(nameof(WorkdayStopTime));
            }
        }


		#endregion

		#region NavigationProperties
		#endregion

		#region Options
		public static class Options
		{
                public struct AllowEmailCredentials
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
			    public struct D365AutoInstallAttemptStatus
                {
					public const int NotAttempted = 0;
					public const int AutoInstalled = 1;
					public const int AlreadyInstalled = 2;
					public const int TeamsAdminBlocked = 3;
					public const int Unauthorized = 4;
					public const int NoSolution = 5;
					public const int NoGraphAPI = 6;
					public const int ResourceDisabled = 7;
                }
			    public struct DataValidationModeForExportToExcel
                {
					public const int Full = 0;
					public const int None = 1;
                }
			    public struct DefaultSearchExperience
                {
					public const int RelevanceSearch = 0;
					public const int CategorizedSearch = 1;
					public const int UseLastSearch = 2;
					public const int CustomSearch = 3;
                }
			    public struct EntityFormMode
                {
					public const int OrganizationDefault = 0;
					public const int ReadOptimized = 1;
					public const int Edit = 2;
                }
                public struct GetStartedPaneContentEnabled
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct IgnoreUnsolicitedEmail
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
			    public struct IncomingEmailFilteringMethod
                {
					public const int AllEmailMessages = 0;
					public const int EmailMessagesInResponseToDynamics365Email = 1;
					public const int EmailMessagesFromDynamics365Leads_ContactsAndAccounts = 2;
					public const int EmailMessagesFromDynamics365RecordsThatAreEmailEnabled = 3;
					public const int NoEmailMessages = 4;
                }
                public struct IsAppsForCrmAlertDismissed
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct IsAutoDataCaptureEnabled
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct IsDefaultCountryCodeCheckEnabled
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct IsDuplicateDetectionEnabledWhenGoingOnline
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct IsEmailConversationViewEnabled
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct IsGuidedHelpEnabled
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct IsResourceBookingExchangeSyncEnabled
                {
                    public const bool Disabled = false;
                    public const bool Enabled = true;
                }
                public struct IsSendAsAllowed
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
			    public struct ReleaseChannel
                {
					public const int None = 0;
					public const int SemiAnnualChannelOverride = 1;
					public const int MonthlyChannelOverride = 2;
					public const int InnerChannelOverride = 3;
                }
			    public struct ReportScriptErrors
                {
					public const int AskMeForPermissionToSendAnErrorReportToMicrosoft = 1;
					public const int AutomaticallySendAnErrorReportToMicrosoftWithoutAskingMeForPermission = 2;
					public const int NeverSendAnErrorReportToMicrosoftAboutMicrosoftDynamics365 = 3;
                }
                public struct ShowWeekNumber
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct SplitViewState
                {
                    public const bool Collapsed = false;
                    public const bool Expanded = true;
                }
                public struct SyncContactCompany
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct TryToggleStatus
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct UseCrmFormForAppointment
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct UseCrmFormForContact
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct UseCrmFormForEmail
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct UseCrmFormForTask
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct UseImageStrips
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
			    public struct VisualizationPaneLayout
                {
					public const int TopBottom = 0;
					public const int SideBySide = 1;
                }
		}
		#endregion

		#region LogicalNames
		public static class LogicalNames
		{
				public const string SystemUserId = "systemuserid";
				public const string AddressBookSyncInterval = "addressbooksyncinterval";
				public const string AdvancedFindStartupMode = "advancedfindstartupmode";
				public const string AllowEmailCredentials = "allowemailcredentials";
				public const string AMDesignator = "amdesignator";
				public const string AutoCaptureUserStatus = "autocaptureuserstatus";
				public const string AutoCreateContactOnPromote = "autocreatecontactonpromote";
				public const string BusinessUnitId = "businessunitid";
				public const string CalendarType = "calendartype";
				public const string CreatedBy = "createdby";
				public const string CreatedOn = "createdon";
				public const string CreatedOnBehalfBy = "createdonbehalfby";
				public const string CurrencyDecimalPrecision = "currencydecimalprecision";
				public const string CurrencyFormatCode = "currencyformatcode";
				public const string CurrencySymbol = "currencysymbol";
				public const string D365AutoInstallAttemptStatus = "d365autoinstallattemptstatus";
				public const string DataValidationModeForExportToExcel = "datavalidationmodeforexporttoexcel";
				public const string DateFormatCode = "dateformatcode";
				public const string DateFormatString = "dateformatstring";
				public const string DateSeparator = "dateseparator";
				public const string DecimalSymbol = "decimalsymbol";
				public const string DefaultCalendarView = "defaultcalendarview";
				public const string DefaultCountryCode = "defaultcountrycode";
				public const string DefaultDashboardId = "defaultdashboardid";
				public const string DefaultSearchExperience = "defaultsearchexperience";
				public const string EmailPassword = "emailpassword";
				public const string EmailUsername = "emailusername";
				public const string EntityFormMode = "entityformmode";
				public const string FullNameConventionCode = "fullnameconventioncode";
				public const string GetStartedPaneContentEnabled = "getstartedpanecontentenabled";
				public const string HelpLanguageId = "helplanguageid";
				public const string HomepageArea = "homepagearea";
				public const string HomepageLayout = "homepagelayout";
				public const string HomepageSubarea = "homepagesubarea";
				public const string IgnoreUnsolicitedEmail = "ignoreunsolicitedemail";
				public const string IncomingEmailFilteringMethod = "incomingemailfilteringmethod";
				public const string IsAppsForCrmAlertDismissed = "isappsforcrmalertdismissed";
				public const string IsAutoDataCaptureEnabled = "isautodatacaptureenabled";
				public const string IsDefaultCountryCodeCheckEnabled = "isdefaultcountrycodecheckenabled";
				public const string IsDuplicateDetectionEnabledWhenGoingOnline = "isduplicatedetectionenabledwhengoingonline";
				public const string IsEmailConversationViewEnabled = "isemailconversationviewenabled";
				public const string IsGuidedHelpEnabled = "isguidedhelpenabled";
				public const string IsResourceBookingExchangeSyncEnabled = "isresourcebookingexchangesyncenabled";
				public const string IsSendAsAllowed = "issendasallowed";
				public const string LastAlertsViewedTime = "lastalertsviewedtime";
				public const string LastModifiedTimeForViewPersonalizationSettings = "lastmodifiedtimeforviewpersonalizationsettings";
				public const string LocaleId = "localeid";
				public const string LongDateFormatCode = "longdateformatcode";
				public const string ModifiedBy = "modifiedby";
				public const string ModifiedOn = "modifiedon";
				public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
				public const string NegativeCurrencyFormatCode = "negativecurrencyformatcode";
				public const string NegativeFormatCode = "negativeformatcode";
				public const string NextTrackingNumber = "nexttrackingnumber";
				public const string NumberGroupFormat = "numbergroupformat";
				public const string NumberSeparator = "numberseparator";
				public const string OfflineSyncInterval = "offlinesyncinterval";
				public const string OutlookSyncInterval = "outlooksyncinterval";
				public const string PagingLimit = "paginglimit";
				public const string PersonalizationSettings = "personalizationsettings";
				public const string PMDesignator = "pmdesignator";
				public const string PreferredSolution = "preferredsolution";
				public const string PricingDecimalPrecision = "pricingdecimalprecision";
				public const string ReleaseChannel = "releasechannel";
				public const string ReportScriptErrors = "reportscripterrors";
				public const string ResourceBookingExchangeSyncVersion = "resourcebookingexchangesyncversion";
				public const string SelectedGlobalFilterId = "selectedglobalfilterid";
				public const string ShowWeekNumber = "showweeknumber";
				public const string SplitViewState = "splitviewstate";
				public const string SyncContactCompany = "synccontactcompany";
				public const string TableScopedDVSearchFeatureTeachingBubbleViews = "tablescopeddvsearchfeatureteachingbubbleviews";
				public const string TableScopedDVSearchQuickFindTeachingBubbleViews = "tablescopeddvsearchquickfindteachingbubbleviews";
				public const string TimeFormatCode = "timeformatcode";
				public const string TimeFormatString = "timeformatstring";
				public const string TimeSeparator = "timeseparator";
				public const string TimeZoneBias = "timezonebias";
				public const string TimeZoneCode = "timezonecode";
				public const string TimeZoneDaylightBias = "timezonedaylightbias";
				public const string TimeZoneDaylightDay = "timezonedaylightday";
				public const string TimeZoneDaylightDayOfWeek = "timezonedaylightdayofweek";
				public const string TimeZoneDaylightHour = "timezonedaylighthour";
				public const string TimeZoneDaylightMinute = "timezonedaylightminute";
				public const string TimeZoneDaylightMonth = "timezonedaylightmonth";
				public const string TimeZoneDaylightSecond = "timezonedaylightsecond";
				public const string TimeZoneDaylightYear = "timezonedaylightyear";
				public const string TimeZoneStandardBias = "timezonestandardbias";
				public const string TimeZoneStandardDay = "timezonestandardday";
				public const string TimeZoneStandardDayOfWeek = "timezonestandarddayofweek";
				public const string TimeZoneStandardHour = "timezonestandardhour";
				public const string TimeZoneStandardMinute = "timezonestandardminute";
				public const string TimeZoneStandardMonth = "timezonestandardmonth";
				public const string TimeZoneStandardSecond = "timezonestandardsecond";
				public const string TimeZoneStandardYear = "timezonestandardyear";
				public const string TrackingTokenId = "trackingtokenid";
				public const string TransactionCurrencyId = "transactioncurrencyid";
				public const string TryToggleSets = "trytogglesets";
				public const string TryToggleStatus = "trytogglestatus";
				public const string UILanguageId = "uilanguageid";
				public const string UseCrmFormForAppointment = "usecrmformforappointment";
				public const string UseCrmFormForContact = "usecrmformforcontact";
				public const string UseCrmFormForEmail = "usecrmformforemail";
				public const string UseCrmFormForTask = "usecrmformfortask";
				public const string UseImageStrips = "useimagestrips";
				public const string UserProfile = "userprofile";
				public const string VersionNumber = "versionnumber";
				public const string VisualizationPaneLayout = "visualizationpanelayout";
				public const string WorkdayStartTime = "workdaystarttime";
				public const string WorkdayStopTime = "workdaystoptime";
		}
		#endregion

		#region Relations
        public static class Relations
        {
            public static class OneToMany
            {
            }

            public static class ManyToOne
            {
				public const string BusinessUnitUserSettings = "business_unit_user_settings";
				public const string LkUsersettingsCreatedonbehalfby = "lk_usersettings_createdonbehalfby";
				public const string LkUsersettingsModifiedonbehalfby = "lk_usersettings_modifiedonbehalfby";
				public const string LkUsersettingsbaseCreatedby = "lk_usersettingsbase_createdby";
				public const string LkUsersettingsbaseModifiedby = "lk_usersettingsbase_modifiedby";
				public const string TransactioncurrencyUsersettings = "transactioncurrency_usersettings";
				public const string UserSettings_ = "user_settings";
				public const string UserSettingsPreferredSolution = "user_settings_preferred_solution";
            }

            public static class ManyToMany
            {
            }
        }

        #endregion

		#region Methods
        public static UserSettings Retrieve(IOrganizationService service, Guid id)
        {
            return Retrieve(service,id, new ColumnSet(true));
        }

        public static UserSettings Retrieve(IOrganizationService service, Guid id, ColumnSet columnSet)
        {
            return service.Retrieve("usersettings", id, columnSet).ToEntity<UserSettings>();
        }

        public UserSettings GetChangedEntity()
        {
            if (_trackChanges)
            {
                var attr = new AttributeCollection();
                foreach (var attrName in _changedProperties.Value.Select(changedProperty => ((AttributeLogicalNameAttribute) GetType().GetProperty(changedProperty).GetCustomAttribute(typeof (AttributeLogicalNameAttribute))).LogicalName).Where(attrName => Contains(attrName)))
                {
                    attr.Add(attrName,this[attrName]);
                }
                return new  UserSettings(Id) {Attributes = attr };
            }
            return this;
        }
        #endregion
	}

	#region Context
	public partial class DataContext
	{
		public IQueryable<UserSettings> UserSettingsSet
		{
			get
			{
				return CreateQuery<UserSettings>();
			}
		}
	}
	#endregion
}
