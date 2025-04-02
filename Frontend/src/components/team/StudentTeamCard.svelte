<script lang="ts">
	import type { Team } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import * as Table from '$lib/components/ui/table';

	export let team: Team | undefined;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-4 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="40"
				height="40"
				src="https://img.icons8.com/fluency/480/group.png"
				alt="knowledge-sharing"
			/>
			{#if team}
				<Card.Title class="m-0 ml-2 text-2xl">Team {team.teamNr}</Card.Title>
			{:else}
				<Card.Title class="m-0 ml-2 text-2xl">Team</Card.Title>
			{/if}
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		{#if team}
			<Table.Root>
				<Table.Header>
					<Table.Row>
						<Table.Head class="h-0 px-6 font-bold text-black">Name</Table.Head>
						<Table.Head class="h-0 px-6 font-bold text-black">Email</Table.Head>
					</Table.Row>
				</Table.Header>
				<Table.Body>
					{#each team.students as student}
						<Table.Row>
							<Table.Cell class="px-6">
								{student.name}
							</Table.Cell>
							<Table.Cell class="px-6">
								{student.email}
							</Table.Cell>
						</Table.Row>
					{/each}
				</Table.Body>
			</Table.Root>
		{:else}
			<p class="py-4 text-center">Student has no team</p>
		{/if}
	</Card.Content>
</Card.Root>
