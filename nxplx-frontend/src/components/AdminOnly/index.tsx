import { h, VNode } from "preact";
import { useMemo } from "preact/hooks";
import { connect } from "unistore/preact";

const AdminOnly = ({ children }: { children: VNode[] | VNode }) => {
    // @ts-ignore
    const Element =  useMemo(() => connect(['isAdmin'], [])(({ isAdmin }) => isAdmin ? children : null), [children]);
    return <Element/>;
}
export default AdminOnly;
