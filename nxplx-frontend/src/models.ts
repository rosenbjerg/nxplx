export interface Library {
    id:number
    name:string
    kind:string
    language:string
    path?:string
}
export interface User {
    username:string
    email:string
    isAdmin:boolean
    passwordChanged:boolean
    libraries:number[]
}

export interface Info { id:number; title:string; poster:string; kind:'film'|'series' }
