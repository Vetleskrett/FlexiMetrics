<script lang="ts">
	import type { Team } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import { Progress } from '$lib/components/ui/progress/index.js';

	export let teams: Team[], courseId: string;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Content class="p-0">
		<Table.Root class="h-full">
			<Table.Header>
				<Table.Row>
					<Table.Head class="flex h-8 items-center justify-center px-6 font-bold text-black">
						Team
					</Table.Head>
					<Table.Head class="h-8 px-6 font-bold text-black">Members</Table.Head>
					<Table.Head class="h-8 w-[250px] px-6 font-bold text-black">Progress</Table.Head>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each teams as team}
					<Table.Row class="text-base ">
						<Table.Cell class="flex h-full items-center justify-center px-6">
							<p>{team.teamNr}</p>
						</Table.Cell>
						<Table.Cell class="px-6">
							{#each team.students as student}
								<p>• {student.email}</p>
							{/each}
						</Table.Cell>
						<Table.Cell class="px-6">
							<Progress value={team.complete} />
						</Table.Cell>
					</Table.Row>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
