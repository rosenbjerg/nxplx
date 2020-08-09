import { h, VNode } from "preact";
import { useState } from "preact/hooks";

interface Props {
    title: string|VNode
    startOpen?: boolean
    children: VNode[]|VNode
}
const Collapsible = ({ children, startOpen, title }: Props) => {
    const [collapsed, setCollapsed] = useState(!startOpen);
    return (
        <div>
            <h3>
                <button class="material-icons noborder" onClick={collapsed ? () => setCollapsed(false) : () => setCollapsed(true)}>{collapsed ? 'expand_more' : 'expand_less'}</button>
                {title}
            </h3>
            {collapsed || children}
        </div>
    );
}
export default Collapsible;
