# prn231-bird-trading-platform

Remember to change Connection Strings in "appsettings.json" file of the API project.

Constants:

*UserAccount:
- Role:
	+ CUSTOMER: Customer
	+ STORE_STAFF: Store staff
	+ ADMIN: Administrator
- Status: 
	+ 1: Active
	+ 0: Inactive (Blocked)

*RegisterDTO:
- StoreId: leave null or 0 => Customer's store Id will be null
- Phone: Regex will check for Vietnamese phone number "0931856541", "+84931856541", "+840931856541"
- CreateNewStore:
	+ 1: Check for NewStoreName and NewStoreAddress and then add a new store entity upon STORE_STAFF account registration
	+ 0: For register new CUSTOMER account or STAFF_STORE account with exist Store (from dropdown list).

