<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import { Input } from '$lib/components/ui/input';
	import ArrowDownToline from 'lucide-svelte/icons/arrow-down-to-line';
	import Save from 'lucide-svelte/icons/save';
	import Undo2 from 'lucide-svelte/icons/undo-2';
	import { Separator } from 'src/lib/components/ui/separator';
	import CustomButton from 'src/components/CustomButton.svelte';
	import type { Delivery, DeliveryField, FieldDelivery } from 'src/types';
	import { Checkbox } from 'src/lib/components/ui/checkbox';

	export let deliveryFields: DeliveryField[];
	let originalDeliveries: Delivery[];
	export { originalDeliveries as deliveries };

	const deliveryTypes = new Map<string, string>(
		deliveryFields.map((field) => [field.id, field.type])
	);

	let deliveries: Delivery[] = structuredClone(originalDeliveries);
	let isAnyChanges = false;

	const teamsChanges = new Map<string, Map<string, FieldDelivery>>(
		deliveries.map((delivery) => [delivery.teamId, new Map<string, FieldDelivery>()])
	);

	const onChange = (teamId: string, field: FieldDelivery) => {
		isAnyChanges = true;
		teamsChanges.get(teamId)?.set(field.fieldId, field);
	};

	const onSubmit = () => {
		console.log('Changes to send to backend:');

		teamsChanges.forEach((teamChanges, teamId) => {
			const changes = teamChanges.values().toArray();

			if (changes.length > 0) {
				console.log(`Team ${teamId}`, changes);
			}
		});

		isAnyChanges = false;
		teamsChanges.forEach((team) => team.clear());
	};

	const undoChanges = () => {
		deliveries = structuredClone(originalDeliveries);
		isAnyChanges = false;
		teamsChanges.forEach((team) => team.clear());
	};
</script>

<form method="POST" on:submit|preventDefault={onSubmit}>
	<Card.Root class="w-[1080px] overflow-hidden p-0">
		<Card.Content class="p-0">
			<Table.Root>
				<Table.Header>
					<Table.Row>
						<Table.Head class="h-0 pt-4 font-bold text-black">Team</Table.Head>
						{#each deliveryFields as field}
							<Table.Head class="h-0 pt-4 font-bold text-black">
								{field.name}
							</Table.Head>
							{#if field.type == 'File'}
								<Table.Head class="h-0 pt-4 font-bold text-black"
									>Upload new [{field.name}]</Table.Head
								>
							{/if}
						{/each}
					</Table.Row>
				</Table.Header>
				<Table.Body>
					{#each deliveries as delivery (delivery.teamId)}
						<Table.Row>
							<Table.Cell>
								{delivery.teamId}
							</Table.Cell>
							{#each delivery.fields as field (field.fieldId)}
								{@const type = deliveryTypes.get(field.fieldId)}
								<Table.Cell>
									{#if type == 'String'}
										<Input
											type="text"
											on:change={() => onChange(delivery.teamId, field)}
											bind:value={field.value}
										/>
									{:else if type == 'Integer'}
										<Input
											type="number"
											class="w-24"
											on:change={() => onChange(delivery.teamId, field)}
											bind:value={field.value}
										/>
									{:else if type == 'Boolean'}
										<Checkbox
											class="m-auto  rounded-[0.25rem]"
											on:click={() => onChange(delivery.teamId, field)}
											bind:checked={field.value}
										/>
									{:else if type == 'File'}
										<a
											href="TODO: add url here"
											download="TODO"
											class="flex items-center text-blue-500"
										>
											<ArrowDownToline size="20" />
											{field.value}
										</a>
									{:else}
										{field.value}
									{/if}
								</Table.Cell>
								{#if type == 'File'}
									<Table.Cell>
										<input
											bind:files={field.value}
											type="file"
											class="flex h-10 w-48 rounded-sm border border-input bg-white px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
											on:change={() => onChange(delivery.teamId, field)}
										/>
									</Table.Cell>
								{/if}
							{/each}
						</Table.Row>
					{/each}
				</Table.Body>
			</Table.Root>
		</Card.Content>

		{#if isAnyChanges}
			<Card.Footer class="flex flex-col items-end p-0">
				<Separator />

				<div class="m-4 flex gap-4">
					<CustomButton color="yellow" on:click={undoChanges}>
						<Undo2 size="20" />
						<p>Undo changes</p>
					</CustomButton>
					<CustomButton color="submit">
						<Save size="20" />
						<p>Save</p>
					</CustomButton>
				</div>
			</Card.Footer>
		{/if}
	</Card.Root>
</form>
