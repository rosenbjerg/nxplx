import { h, VNode } from 'preact';
import { useCallback } from 'preact/hooks';

interface FormProps {
	onSubmit: (formData: FormData) => Promise<boolean> | boolean;
	children: VNode[];
}

const Form = ({ children, onSubmit, ...rest }: FormProps) => {
	const submit = useCallback((ev) => {
		ev.preventDefault();
		const formdata = new FormData(ev.target);
		const reset = !onSubmit(formdata);
		if (reset) ev.target.reset();
	}, []);

	return (
		<form {...rest} onSubmit={submit}>
			{children}
		</form>
	);
};
export default Form;