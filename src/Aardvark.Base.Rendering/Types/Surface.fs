﻿namespace Aardvark.Base

open System
open FShade
open FSharp.Data.Adaptive

[<AllowNullLiteral>]
type ISurface = interface end

[<AllowNullLiteral>]
type IDisposableSurface =
    inherit ISurface
    inherit IDisposable

[<RequireQualifiedAccess>]
type Surface =
    | FShadeSimple of Effect
    | FShade of (EffectConfig -> EffectInputLayout * aval<Imperative.Module>)
    | Backend of ISurface
    | None