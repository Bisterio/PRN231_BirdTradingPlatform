# prn231-bird-trading-platform

Remember to change Connection Strings in "appsettings.json" file of the API project.

Constants:

*UserAccount:
- Role:
	+ CUSTOMER: Customer
	+ STORE: Store
	+ ADMIN: Administrator
- Status: 
	+ 1: Active
	+ 0: Inactive (Blocked)

*RegisterDTO:
- StoreId: leave null or 0 => Customer's store Id will be null
- Phone: Regex will check for Vietnamese phone number "0931856541", "+84931856541", "+840931856541"
- CreateNewStore:
	+ 1: Check for NewStoreName and NewStoreAddress and then add a new store entity upon STORE_STAFF account registration
	+ 0: For register new CUSTOMER account or STORE account with exist Store (from dropdown list).

*Order Status
ORDER STATUS
0: Ko query đc
1: Waiting for approval
2: Delivered
3: Packing
4: Delivering
5: Waiting for cancel approval
6: Cancelled by store
7: Cancelled by customer
8: Waiting for refund approval
9: Refunded

ORDER IS_REPORTED STATUS
0: Ko query
1: Not yet (Default)
2: Is reported
3: Resolved

