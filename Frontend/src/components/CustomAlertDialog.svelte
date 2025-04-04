<script lang="ts">
	import * as AlertDialog from '$lib/components/ui/alert-dialog';

	export let show: boolean = false;
	export let description: string;
	export let onConfirm: () => void;
	export let action: string;

	export let title: string = 'Are you absolutely sure?';
</script>

<AlertDialog.Root bind:open={show}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>{title}</AlertDialog.Title>
			<AlertDialog.Description>{description}</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
			<AlertDialog.Action on:click={onConfirm}>{action}</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
