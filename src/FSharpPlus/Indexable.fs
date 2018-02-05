﻿namespace FSharpPlus.Control

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Text
open System.Collections.Generic
open FSharpPlus.Internals
open FSharpPlus.Internals.Prelude

#nowarn "77"
// Warn FS0077 -> Member constraints with the name 'get_Item' are given special status by the F# compiler as certain .NET types are implicitly augmented with this member. This may result in runtime failures if you attempt to invoke the member constraint from your own code.
// Those .NET types are string and array but they are explicitely handled here.

[<Extension;Sealed>]
type Item =
    inherit Default1
    static member inline       Item (x: '``Indexable<'T>`` , k        , [<Optional>]_impl: Default1) = (^``Indexable<'T>`` : (member get_Item : _ -> 'T) x, k) : 'T
    static member inline       Item (_: 'T when 'T: null and 'T: struct, _,         _impl: Default1) = ()
    [<Extension>]static member Item (x: string             , n        , [<Optional>]_impl: Item    ) = x.[n]
    [<Extension>]static member Item (x: StringBuilder      , n        , [<Optional>]_impl: Item    ) = x.ToString().[n]
    [<Extension>]static member Item (x: 'T []              , n        , [<Optional>]_impl: Item    ) = x.[n]       : 'T
    [<Extension>]static member Item (x: 'T [,]             , (i,j)    , [<Optional>]_impl: Item    ) = x.[i,j]     : 'T
    [<Extension>]static member Item (x: 'T [,,]            , (i,j,k)  , [<Optional>]_impl: Item    ) = x.[i,j,k]   : 'T
    [<Extension>]static member Item (x: 'T [,,,]           , (i,j,k,l), [<Optional>]_impl: Item    ) = x.[i,j,k,l] : 'T

    static member inline Invoke (n:'K) (source:'``Indexed<'T>``)  :'T =
        let inline call_2 (a:^a, b:^b, n) = ((^a or ^b) : (static member Item: _*_*_ -> _) b, n, a)
        let inline call (a:'a, b:'b, n) = call_2 (a, b, n)
        call (Unchecked.defaultof<Item>, source, n)


[<Extension;Sealed>]
type TryItem =
    inherit Default1
    static member              TryItem (x: IDictionary<'K,'T>, k        , [<Optional>]_impl: Default2) = tupleToOption (x.TryGetValue k)                                  : 'T option
    static member inline       TryItem (x: '``Indexable<'T>``, k        , [<Optional>]_impl: Default1) = (^``Indexable<'T>`` : (static member TryItem : _ * _ -> _) k, x) : 'T option
    static member inline       TryItem (_: 'T when 'T: null and 'T: struct, _       , _impl: Default1) = ()
    [<Extension>]static member TryItem (x: string            , n        , [<Optional>]_impl: TryItem ) = if n >= 0 && n < x.Length then Some (x.[n]) else None
    [<Extension>]static member TryItem (x: StringBuilder     , n        , [<Optional>]_impl: TryItem ) = if n >= 0 && n < x.Length then Some ((string x).[n]) else None
    [<Extension>]static member TryItem (x: 'a []             , n        , [<Optional>]_impl: TryItem ) = if n >= x.GetLowerBound 0 && n <= x.GetUpperBound 0 then Some x.[n] else None : 'a option
    [<Extension>]static member TryItem (x: 'a [,]            , (i,j)    , [<Optional>]_impl: TryItem ) = if (i, j)       >= (x.GetLowerBound 0, x.GetLowerBound 1                                      ) && (i, j)       <= (x.GetUpperBound 0, x.GetUpperBound 1                                      ) then Some x.[i,j]     else None : 'a option
    [<Extension>]static member TryItem (x: 'a [,,]           , (i,j,k)  , [<Optional>]_impl: TryItem ) = if (i, j, k)    >= (x.GetLowerBound 0, x.GetLowerBound 1, x.GetLowerBound 2)                    && (i, j, k)    <= (x.GetUpperBound 0, x.GetUpperBound 1, x.GetUpperBound 2                   ) then Some x.[i,j,k]   else None : 'a option
    [<Extension>]static member TryItem (x: 'a [,,,]          , (i,j,k,l), [<Optional>]_impl: TryItem ) = if (i, j, k, l) >= (x.GetLowerBound 0, x.GetLowerBound 1, x.GetLowerBound 2, x.GetLowerBound 3) && (i, j, k, l) <= (x.GetUpperBound 0, x.GetUpperBound 1, x.GetUpperBound 2, x.GetUpperBound 3) then Some x.[i,j,k,l] else None : 'a option
    [<Extension>]static member TryItem (x: 'a ResizeArray    , n        , [<Optional>]_impl: TryItem ) = if n >= 0 && n < x.Count then Some x.[n] else None
    [<Extension>]static member TryItem (x: list<'a>          , n        , [<Optional>]_impl: TryItem ) = List.tryItem n x
    [<Extension>]static member TryItem (x: Map<'K,'T>        , k        , [<Optional>]_impl: TryItem ) = x.TryFind k : 'T option

    static member inline Invoke (n: 'K) (source: '``Indexed<'T>``) : 'T option =
        let inline call_2 (a:^a, b:^b, n) = ((^a or ^b) : (static member TryItem: _*_*_ -> _) b, n, a)
        let inline call (a:'a, b:'b, n) = call_2 (a, b, n)
        call (Unchecked.defaultof<TryItem>, source, n)



type MapIndexed =
    static member MapIndexed (x: Id<'T>     , f: _->'T->'U , [<Optional>]_impl: MapIndexed) = f () x.getValue
    static member MapIndexed (x: seq<'T>    , f            , [<Optional>]_impl: MapIndexed) = Seq.mapi   f x
    static member MapIndexed (x: list<'T>   , f            , [<Optional>]_impl: MapIndexed) = List.mapi  f x
    static member MapIndexed (x: 'T []      , f            , [<Optional>]_impl: MapIndexed) = Array.mapi f x
    static member MapIndexed ((k: 'K, a: 'T), f            , [<Optional>]_impl: MapIndexed) = (k, ((f k a) : 'U))
    static member MapIndexed (g             , f: 'K->'T->'U, [<Optional>]_impl: MapIndexed) = fun x -> f x (g x)
    static member MapIndexed (x: Map<'K,'T> , f            , [<Optional>]_impl: MapIndexed) = Map.map f x : Map<'K,'U>

    static member inline Invoke (mapping: 'K->'T->'U) (source: '``Indexable<'T>``) =
        let inline call_3 (a:^a, b:^b, _:^c, f) = ((^a or ^b or ^c) : (static member MapIndexed: _*_*_ -> _) b, f, a)
        let inline call (a:'a, b:'b, f) = call_3 (a, b, Unchecked.defaultof<'r>, f) :'r
        call (Unchecked.defaultof<MapIndexed>,   source, mapping)   : '``Indexable<'U>``


type IterateIndexed =
    static member IterateIndexed (x: Id<'T>    , f: _->'T->unit, [<Optional>]_impl: IterateIndexed) = f () x.getValue
    static member IterateIndexed (x: seq<'T>   , f             , [<Optional>]_impl: IterateIndexed) = Seq.iteri   f x
    static member IterateIndexed (x: list<'T>  , f             , [<Optional>]_impl: IterateIndexed) = List.iteri  f x
    static member IterateIndexed (x: 'T []     , f             , [<Optional>]_impl: IterateIndexed) = Array.iteri f x
    static member IterateIndexed (x: Map<'K,'T>, f             , [<Optional>]_impl: IterateIndexed) = Map.iter f x

    static member inline Invoke (action: 'K->'T->unit) (source: '``Indexable<'T>``)        =
        let inline call_2 (a:^a, b:^b, f) = ((^a or ^b) : (static member IterateIndexed: _*_*_ -> _) b, f, a)
        let inline call (a:'a, b:'b, f) = call_2 (a, b, f)
        call (Unchecked.defaultof<IterateIndexed>,  source, action) : unit



type FoldIndexed =
    static member        FoldIndexed (x: seq<_>    , f, z, _impl: FoldIndexed) = x |> Seq.fold   (fun (p, i) t -> (f p i t, i + 1)) (z, 0) |> fst
    static member        FoldIndexed (x: list<_>   , f, z, _impl: FoldIndexed) = x |> List.fold  (fun (p, i) t -> (f p i t, i + 1)) (z, 0) |> fst
    static member        FoldIndexed (x: _ []      , f, z, _impl: FoldIndexed) = x |> Array.fold (fun (p, i) t -> (f p i t, i + 1)) (z, 0) |> fst
    static member        FoldIndexed (_: Map<'k,'t>, f, z, _impl: FoldIndexed) = Map.fold f z

    static member inline Invoke (folder:'State->'Key->'T->'State) (state:'State) (foldable:'``Foldable<'T>``) : 'State =
        let inline call_2 (a:^a, b:^b, f, z) = ((^a or ^b) : (static member FoldIndexed: _*_*_*_ -> _) b, f, z, a)
        let inline call (a:'a, b:'b, f, z) = call_2 (a, b, f, z)
        call (Unchecked.defaultof<FoldIndexed>, foldable, folder, state)


type TraverseIndexed =
    static member inline TraverseIndexed ((k: 'K, a: 'T), f , [<Optional>]_output: 'R, [<Optional>]_impl: TraverseIndexed) : 'R = Map.Invoke ((fun x y -> (x, y)) k) (f k a)
    static member inline TraverseIndexed (a: Tuple<_>   , f , [<Optional>]_output: 'R, [<Optional>]_impl: TraverseIndexed) : 'R = Map.Invoke Tuple (f () a.Item1)

    static member inline Invoke f t =
        let inline call_3 (a:^a, b:^b, c:^c, f) = ((^a or ^b or ^c) : (static member TraverseIndexed: _*_*_*_ -> _) b, f, c, a)
        let inline call (a:'a, b:'b, f) = call_3 (a, b, Unchecked.defaultof<'r>, f) : 'r
        call (Unchecked.defaultof<TraverseIndexed>, t, f)