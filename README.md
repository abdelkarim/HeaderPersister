`HeaderPersister` is a small utility class that persist the header of individual `GroupItem`s while scrolling. It is not tied to any particular `ItemsControl` derived class, I confirmed a correct behavior for `ListBox`, `ListView` and `TreeView`.

![header-persister](https://cloud.githubusercontent.com/assets/1153480/9800714/7a6e4c62-5806-11e5-867f-1fe2b9d00f9b.gif)

Usage
=====
The usage of the `HeaderPersister` is straightforward, make sure to:

+ Bind to your `CollectionViewSource` instance and define your `GroupDescriptions` collection.
+ Set the attached property `HeaderPersister.IsEnabled` to `true`.
+ Duplicate the visual tree of the `GroupItem` header in the `HeaderPersister.HeaderTemplate` attached property.

For more information, check the following blog post: http://blog.qarim.net/2013/08/16/wpf-persistent-group-headers-for-the-listbox-control/
