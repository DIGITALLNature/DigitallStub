// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Tests.Fixtures;

public partial class Account
{
    [AttributeLogicalName("new_accountcategorycodemultiple")]
    public OptionSetValueCollection AccountCategoryCodeMultiple
    {
        get
        {
            return GetAttributeValue<OptionSetValueCollection>("new_accountcategorycodemultiple");
        }
        set
        {
            OnPropertyChanging(nameof(AccountCategoryCodeMultiple));
            SetAttributeValue("new_accountcategorycodemultiple", value);
            OnPropertyChanged(nameof(AccountCategoryCodeMultiple));
        }
    }
}
