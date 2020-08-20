import { h } from "preact";

interface Props {
    border?: boolean
    onClick?: () => any
    icon: string
    style?: string
    outerClass?: string
}

const IconButton = ({ border, icon, outerClass = '', ...rest }: Props) => {
    return (
        <button class={`material-icons ${border ? '' : 'noborder'} ${outerClass}`} {...rest}>{icon}</button>
    )
}

export default IconButton;
